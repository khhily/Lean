using System;

namespace WGX.Common
{
    public interface ILog
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        void Log(Exception ex);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        void Log(string msg);

        /// <summary>
        /// 
        /// </summary>
        void Config();

        #region 自定义操作日志接口

        /// <summary>
        /// 写入登录系统日志
        /// </summary>
        /// <param name="msg"></param>
        void LogSystemLogin(string msg);

        /// <summary>
        /// 写入模块访问日志
        /// </summary>
        /// <param name="msg"></param>
        void LogModuleVisit(string msg);

        /// <summary>
        /// 写入操作异常日志
        /// </summary>
        /// <param name="msg"></param>
        void LogOperateExeption(string msg);

        /// <summary>
        /// 写入业务操作日志
        /// </summary>
        /// <param name="msg"></param>
        void LogBizOperate(string msg);

        /// <summary>
        /// 写入接口调用日志
        /// </summary>
        /// <param name="msg"></param>
        void LogInterfaceCall(string msg);

        /// <summary>
        /// 越权访问日志
        /// </summary>
        /// <param name="msg"></param>
        void LogNoPower(string msg);

        #endregion

    }
}
