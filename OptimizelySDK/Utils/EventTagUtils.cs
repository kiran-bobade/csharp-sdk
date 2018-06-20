﻿/* 
 * Copyright 2017-2018, Optimizely
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using OptimizelySDK.Logger;

namespace OptimizelySDK.Utils
{
    public class EventTagUtils
    {
        public const string REVENUE_EVENT_METRIC_NAME = "revenue";
        public const string VALUE_EVENT_METRIC_NAME = "value";

        public static object GetRevenueValue(Dictionary<string, object> eventTags, ILogger logger)
        {
            int result = 0;
            bool isCasted = false;
            string logMessage = string.Empty;
            LogLevel logLevel = LogLevel.INFO;

            if (eventTags == null)
            {
                logMessage = "Event tags is undefined.";
                logLevel = LogLevel.DEBUG;
            }
            else if (!eventTags.ContainsKey(REVENUE_EVENT_METRIC_NAME))
            {
                logMessage = "The revenue key is not defined in the event tags.";
                logLevel = LogLevel.DEBUG;
            }
            else if (eventTags[REVENUE_EVENT_METRIC_NAME] == null)
            {
                logMessage = "The revenue key value is not defined in event tags.";
                logLevel = LogLevel.ERROR;
            }
            else if (!int.TryParse(eventTags[REVENUE_EVENT_METRIC_NAME].ToString(), out result))
            {
                logMessage = "Revenue value is not an integer or couldn't be parsed as an integer.";
                logLevel = LogLevel.ERROR;
            }
            else
            {
                isCasted = true;
                logMessage = $"The revenue value {result} will be sent to results.";
            }

            if (logger != null)
                logger.Log(logLevel, logMessage);

            if (isCasted)
                return result;

            return null;
        }

        public static object GetNumericValue(Dictionary<string, object> eventTags, ILogger logger  = null)
        {
            string debugMessage = string.Empty;
            bool isCasted = false;

            float refVar = 0;

            if (eventTags == null)
                debugMessage = "Event tags is undefined.";

            else if (!eventTags.ContainsKey(VALUE_EVENT_METRIC_NAME))
                debugMessage = "The numeric metric key is not in event tags.";

            else if (eventTags[VALUE_EVENT_METRIC_NAME] == null)
                debugMessage = "The numeric metric key value is not defined in event tags.";

            else if (eventTags[VALUE_EVENT_METRIC_NAME] is bool)
                debugMessage = "Provided numeric value is boolean which is an invalid format.";

            else if (!(eventTags[VALUE_EVENT_METRIC_NAME] is int) && !(eventTags[VALUE_EVENT_METRIC_NAME] is string) && !(eventTags[VALUE_EVENT_METRIC_NAME] is float)
                && !(eventTags[VALUE_EVENT_METRIC_NAME] is decimal)
                && !(eventTags[VALUE_EVENT_METRIC_NAME] is double)
                && !(eventTags[VALUE_EVENT_METRIC_NAME] is float))
                debugMessage = "Numeric metric value is not in integer, float, or string form.";

            else
            {
                if (!float.TryParse(eventTags[VALUE_EVENT_METRIC_NAME].ToString(), out refVar))
                    debugMessage = string.Format("Provided numeric value {0} is in an invalid format.", eventTags[VALUE_EVENT_METRIC_NAME]);
                else
                {
                    if (float.IsInfinity(refVar))
                        debugMessage = string.Format("Provided numeric value {0} is in an invalid format.", eventTags[VALUE_EVENT_METRIC_NAME]);
                    else
                        isCasted = true;
                }
            }

            if (logger != null)
            {
                if (isCasted)
                    logger.Log(LogLevel.INFO, string.Format("The numeric metric value {0} will be sent to results.", refVar));
                else
                    if (string.IsNullOrEmpty(debugMessage))
                        logger.Log(LogLevel.ERROR, string.Format("The provided numeric metric value {0} is in an invalid format and will not be sent to results.", eventTags[VALUE_EVENT_METRIC_NAME]));
                    else
                        logger.Log(LogLevel.ERROR, debugMessage);
            }

            object o = refVar;

            if(isCasted && eventTags[VALUE_EVENT_METRIC_NAME] is float)
            {
                // Special case, maximum value when passed and gone through tryparse, it loses precision.
                o = eventTags[VALUE_EVENT_METRIC_NAME];
            }

            return isCasted ? o : null;

        }
    }
}
 