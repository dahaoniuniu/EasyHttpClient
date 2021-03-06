﻿using EasyHttpClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class PathParamAttribute : Attribute, IParameterScopeAttribute
    {
        public PathParamAttribute() { 
        
        }
        public PathParamAttribute(string name)
        {
            this.Name = name;
        }

        public ParameterScope Scope
        {
            get
            {
                return ParameterScope.Path;
            }
        }
        public string Name
        {
            get;
            set;
        }

        internal string[] PathParamNamesFilter { get; set; }


        internal void ProcessParameter(HttpRequestMessageBuilder requestBuilder, object parameterValue)
        {
            var pFormatAttr = requestBuilder.DefaultStringFormatter;
            var processedParameters = Utility.ExtractUrlParameter(this.Name, parameterValue, pFormatAttr, 1);

            foreach (var p in processedParameters.GroupBy(i => i.Key))
            {
                if (PathParamNamesFilter.Contains(p.Key, StringComparer.OrdinalIgnoreCase))
                {
                    requestBuilder.PathParams[p.Key] = string.Join(",", p.Select(i => i.Value));
                }
            };
        }

        public void ProcessParameter(HttpRequestMessageBuilder requestBuilder, ParameterInfo parameterInfo, object parameterValue)
        {

            var pFormatAttr = parameterInfo.GetCustomAttribute<StringFormatAttribute>() ?? requestBuilder.DefaultStringFormatter;
            var processedParameters = Utility.ExtractUrlParameter(this.Name ?? parameterInfo.Name, parameterValue, pFormatAttr,1);

            foreach (var p in processedParameters.GroupBy(i=>i.Key))
            {
                if (PathParamNamesFilter.Contains(p.Key,StringComparer.OrdinalIgnoreCase))
                {
                    requestBuilder.PathParams[p.Key] = string.Join(",", p.Select(i => i.Value));
                }
            };
        }
    }
}
