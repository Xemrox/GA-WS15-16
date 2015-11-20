using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XGA.Helper {

    public abstract class Logger {
        protected bool m_finished = false;
        protected HashSet<string> m_enabledLvl = new HashSet<string>();

        protected Logger() {
            this.m_enabledLvl.Add("log");
            this.m_enabledLvl.Add("result");
        }

        public void EnableLogLvl(string lvl) {
            this.m_enabledLvl.Add(lvl);
        }

        public virtual void Log(string msg, string lvl = "log") {
            if (this.m_finished) throw new Exception("Logger allready finished");
        }

        public virtual void Write(string s) {
            if (this.m_finished) throw new Exception("Logger allready finished");
        }

        public virtual void Finish() {
            this.m_finished = true;
        }
    }

    public class StreamLogger : Logger {
        private readonly StreamWriter m_target;

        public StreamLogger(string target) {
            this.m_target = new StreamWriter(target, false, Encoding.UTF8);
        }

        public override void Log(string msg, string lvl = "log") {
            base.Log(msg, lvl);
            if (this.m_enabledLvl.Contains(lvl)) {
                this.m_target.WriteLine(string.Format("[{1}] {0}", msg, DateTime.Now.ToLongTimeString()));
            }
        }

        public override void Write(string s) {
            base.Write(s);
            this.m_target.Write(s);
        }

        public override void Finish() {
            base.Finish();
            this.m_target.Flush();
            this.m_target.Close();
            this.m_target.Dispose();
        }
    }

    public class EmptyLogger : Logger {
    }

    public class ConsoleLogger : Logger {

        public override void Log(string msg, string lvl = "log") {
            //base.Log(msg, lvl);   //ignore finish
            if (this.m_enabledLvl.Contains(lvl)) {
                Console.WriteLine(msg);
            }
        }

        public override void Write(string s) {
            //base.Write(s);    //ignore finish
            Console.Write(s);
        }
    }
}