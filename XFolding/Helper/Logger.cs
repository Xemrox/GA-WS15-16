using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XGA.Helper {

    public sealed class Logger {
        private bool m_finished = false;
        private readonly StreamWriter m_target;
        private HashSet<string> m_enabledLvl = new HashSet<string>();

        public Logger(string target) {
            this.m_target = new StreamWriter(target, false, Encoding.UTF8);
            this.m_enabledLvl.Add("log");
            this.m_enabledLvl.Add("result");
        }

        public void EnableLogLvl(string lvl) {
            this.m_enabledLvl.Add(lvl);
        }

        public void Log(string msg, string lvl = "log") {
            if (this.m_finished) throw new Exception("Logger allready finished");

            if (this.m_enabledLvl.Contains(lvl)) {
                this.m_target.WriteLine(string.Format("[{1}] {0}", msg, DateTime.Now.ToLongTimeString()));
            } /*
                Console.WriteLine("Blocked lvl: {0} Msg: {1}", lvl, msg);
            */
        }

        public void Write(string s) {
            if (this.m_finished) throw new Exception("Logger allready finished");

            this.m_target.Write(s);
        }

        public void Finish() {
            this.m_target.Flush();
            this.m_target.Close();
            this.m_target.Dispose();
        }
    }
}