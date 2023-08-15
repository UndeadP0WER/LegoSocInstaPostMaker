using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoSocInstaPostMaker.Actions {
    class ActionManager {
        private Stack<Action> actions = new Stack<Action>();
        private Stack<Action> undone = new Stack<Action>();

        
        public void DoAction(Action action) {
            //adds the action to 
            actions.Push(action);
            undone.Clear();
        }

        public Action UndoAction() {
            //if there is an action to undo, returns it for proper handling
            if (actions.Count > 0) {
                Action a = actions.Pop();
                undone.Push(a);
                return a;
            }
            return null;
        }

        public Action RedoAction() {
            //if there is an action to redo, returns it for proper handling
            if (undone.Count > 0) {
                Action a = undone.Pop();
                actions.Push(a);
                return a;
            }
            return null;
        }


    }
}
