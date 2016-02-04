using System;
using log4net;

namespace WGX.Common
{
    public class Logger : ILog
    {
        //1、登录系统日志
        private static readonly string SystemLoginLogName = "SystemLoginLog";
        //2、模块访问日志
        private static readonly string ModuleVisitLogName = "ModuleVisitLog";
        //3、操作异常日志
        private static readonly string OperateExeptionLogName = "OperateExeptionLog";
        //4、业务操作日志
        private static readonly string BizOperateLogName = "BizOperateLog";
        //5、接口调用日志
        private static readonly string InterfaceCallLogName = "InterfaceCallLog";
        //6、越权访问日志
        private static readonly string NoPowerLogName = "NoPowerLog";

        public void Log(Exception ex)
        {
            var logger = LogManager.GetLogger("");
            logger.Error("", ex);
        }

        public void Log(string msg)
        {
            var logger = LogManager.GetLogger("");
            logger.Info(msg);
        }

        public void Config()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// 写入登录系统日志
        /// </summary>
        /// <param name="msg"></param>
        public void LogSystemLogin(string msg)
        {
            var logger = LogManager.GetLogger(SystemLoginLogName);
            logger.Info(msg);
        }

        /// <summary>
        /// 写入模块访问日志
        /// </summary>
        /// <param name="msg"></param>
        public void LogModuleVisit(string msg)
        {
            var logger = LogManager.GetLogger(ModuleVisitLogName);
            logger.Info(msg);
        }

        /// <summary>
        /// 写入操作异常日志
        /// </summary>
        /// <param name="msg"></param>
        public void LogOperateExeption(string msg)
        {
            var logger = LogManager.GetLogger(OperateExeptionLogName);
            logger.Info(msg);
        }

        /// <summary>
        /// 写入业务操作日志
        /// </summary>
        /// <param name="msg"></param>
        public void LogBizOperate(string msg)
        {
            var logger = LogManager.GetLogger(BizOperateLogName);
            logger.Info(msg);
        }

        /// <summary>
        /// 写入接口调用日志
        /// </summary>
        /// <param name="msg"></param>
        public void LogInterfaceCall(string msg)
        {
            var logger = LogManager.GetLogger(InterfaceCallLogName);
            logger.Info(msg);
        }

        /// <summary>
        /// 越权访问日志
        /// </summary>
        /// <param name="msg"></param>
        public void LogNoPower(string msg)
        {
            var logger = LogManager.GetLogger(NoPowerLogName);
            logger.Info(msg);
        }
    }
}
