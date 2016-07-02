using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurGame.Engine
{
    public class EventSystem
    {
        private static EventSystem _instance;

        public static EventSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventSystem();
                }
                return _instance;
            }
        }

        private Dictionary<string, List<Action<object>>> _tokensAndActions = new Dictionary<string, List<Action<object>>>();


        public void RegisterForEvent(string token, Action<object> action)
        {
            if (!_tokensAndActions.ContainsKey(token))
                _tokensAndActions.Add(token, new List<Action<object>>());
            _tokensAndActions[token].Add(action);
        }

        public void Send(string token, object item)
        {
            if (_tokensAndActions.ContainsKey(token))
                foreach (Action<object> x in _tokensAndActions[token])
                    x.Invoke(item);
        }
        public void UnregisterForEvent(string token, Action<object> action)
        {
            if (_tokensAndActions.ContainsKey(token))
                if (_tokensAndActions[token].Contains(action))
                    _tokensAndActions[token].Remove(action);
        }
    }
}
