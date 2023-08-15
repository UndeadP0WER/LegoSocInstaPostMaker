using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoSocInstaPostMaker.Actions {
    public enum ActionType {
        Place,
        Delete
    }

    public class Action {
        public ActionType ActionType { get; }
        public object Data { get; }


        public Action(ActionType type, object data) {
            ActionType = type;
            Data = data;
        }

    }
}
