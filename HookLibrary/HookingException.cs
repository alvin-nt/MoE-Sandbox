using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HookLibrary
{
    [Serializable]
    public class HookingException : SystemException
    {
        private HookData _apiHook;
        private string _apiHookDelegateName;

        /// <summary>
        /// Gets or sets the data describing target function causing the exception.
        /// </summary>
        public HookData ApiHook
        {
            get { return _apiHook; }
            set
            {
                _apiHook = value;
                _apiHookDelegateName = _apiHook.Handler.Method.DeclaringType + " -> " + _apiHook.Handler.Method;
            }
        }

        public HookingException()
        {
        }

        public HookingException(string message)
            : base(message)
        {
        }

        public HookingException(string message, HookData hookData)
            : base(message)
        {
            ApiHook = hookData;
        }

        public HookingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public HookingException(string message, HookData hookData, Exception innerException)
            : base(message, innerException)
        {
            ApiHook = hookData;
        }

        /// <summary>
        /// Creates and returns a <see cref="string"/> representation of the current <see cref="HookingException"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Return base.ToString() with hook target inserted on second line
            var lines = new List<string>(base.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None));
            var extraLine = $"Hook target: {_apiHook}{Environment.NewLine}Hook delegate: {_apiHookDelegateName}";
            if (lines.Count > 1)
                lines.Insert(1, extraLine);
            else
                lines.Add(extraLine);
            var sb = new StringBuilder();
            foreach (var line in lines)
                sb.AppendLine(line);
            return sb.ToString();
        }

        protected HookingException(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            try
            {
                _apiHook = new HookData(
                    info.GetString("HookDescription"),
                    info.GetString("HookTargetLibrary"),
                    info.GetString("HookTargetSymbol"),
                    null, null);
                _apiHookDelegateName = info.GetString("HookDelegate");
            }
            catch (SerializationException)
            {
            }
        }
    }
}