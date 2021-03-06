﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSLE
{
    /// <summary>
    /// 支持有返回值的带 1 个参数的委托注册.
    /// 注意这里和void类型委托注册的用法有些区别：
    /// 这里的类模板第一个参数是返回类型.
    ///    比如有个返回bool型的委托定义如下：
    ///    public class Class {
    ///         public delegate bool BoolParam1Delegate(int param);
    ///    }
    ///    那么注册方式如下：
    ///    env.RegType(new RegHelper_DeleNonVoidAction<bool, int>(typeof(Class.BoolParam1Delegate), "Class.BoolParam1Delegate"));
    /// </summary>
    public class RegHelper_DeleNonVoidAction<ReturnType, T> : RegHelper_Type, ICLS_Type_Dele
    {
        /// <summary>
        /// 有返回值,同时带 1个 参数的委托.
        /// </summary>
        /// <returns></returns>
        public delegate ReturnType NonVoidDelegate(T param);

        public RegHelper_DeleNonVoidAction(Type type, string setkeyword)
            : base(type, setkeyword, true)
        {

        }

        public override object Math2Value(CLS_Content env, char code, object left, CLS_Content.Value right, out CLType returntype)
        {
            returntype = null;

            if (left is DeleEvent)
            {
                DeleEvent info = left as DeleEvent;
                Delegate calldele = null;

                //!--exist bug.
                /*if (right.value is DeleFunction) calldele = CreateDelegate(env.environment, right.value as DeleFunction);
                else if (right.value is DeleLambda) calldele = CreateDelegate(env.environment, right.value as DeleLambda);
                else if (right.value is Delegate) calldele = right.value as Delegate;*/

                object rightValue = right.value;
                if (rightValue is DeleFunction)
                {
                    if (code == '+')
                    {
                        calldele = CreateDelegate(env.environment, rightValue as DeleFunction);
                    }
                    else if (code == '-')
                    {
                        calldele = CreateDelegate(env.environment, rightValue as DeleFunction);
                    }
                }
                else if (rightValue is DeleLambda)
                {
                    if (code == '+')
                    {
                        calldele = CreateDelegate(env.environment, rightValue as DeleLambda);
                    }
                    else if (code == '-')
                    {
                        calldele = CreateDelegate(env.environment, rightValue as DeleLambda);
                    }
                }
                else if (rightValue is Delegate)
                {
                    calldele = rightValue as Delegate;
                }

                if (code == '+')
                {
                    info._event.AddEventHandler(info.source, calldele);
                    //if (!(rightValue is Delegate)) {
                    //    Dele_Map_Delegate.Map(rightValue as IDeleBase, calldele);
                    //}
                    return info;
                }
                else if (code == '-')
                {
                    info._event.RemoveEventHandler(info.source, calldele);
                    //if (!(rightValue is Delegate)) {
                    //    Dele_Map_Delegate.Destroy(rightValue as IDeleBase);
                    //}
                    return info;
                }

            }
            else if (left is Delegate || left == null)
            {
                Delegate info = left as Delegate;
                Delegate calldele = null;
                if (right.value is DeleFunction)
                    calldele = CreateDelegate(env.environment, right.value as DeleFunction);
                else if (right.value is DeleLambda)
                    calldele = CreateDelegate(env.environment, right.value as DeleLambda);
                else if (right.value is Delegate)
                    calldele = right.value as Delegate;
                if (code == '+')
                {
                    return Delegate.Combine(info, calldele); ;
                }
                else if (code == '-')
                {
                    return Delegate.Remove(info, calldele);
                }
            }
            return new NotSupportedException();
        }
        public Delegate CreateDelegate(ICLS_Environment env, DeleFunction delefunc)
        {
            DeleFunction _func = delefunc;
            Delegate _dele = delefunc.cacheFunction(this._type, null);
            if (_dele != null) return _dele;
            NonVoidDelegate dele = delegate(T param)
            {
                var func = _func.calltype.functions[_func.function];
                if (func.expr_runtime != null)
                {
                    CLS_Content content = new CLS_Content(env, true);
                    try
                    {
                        content.DepthAdd();
                        content.CallThis = _func.callthis;
                        content.CallType = _func.calltype;
                        content.function = _func.function;

                        content.DefineAndSet(func._paramnames[0], func._paramtypes[0].type, param);
                        CLS_Content.Value retValue = func.expr_runtime.ComputeValue(content);
                        content.DepthRemove();

                        return (ReturnType)retValue.value;
                    }
                    catch (Exception err)
                    {
                        string errinfo = "Dump Call in:";
                        if (_func.calltype != null) errinfo += _func.calltype.Name + "::";
                        if (_func.function != null) errinfo += _func.function;
                        errinfo += "\n";
                        env.logger.Log(errinfo + content.Dump()); 
                        throw err;
                    }
                }
                return default(ReturnType);
            };
            _dele = Delegate.CreateDelegate(this.type, dele.Target, dele.Method);
            return delefunc.cacheFunction(this._type, _dele);
        }

        public Delegate CreateDelegate(ICLS_Environment env, DeleLambda lambda)
        {
            CLS_Content content = lambda.content.Clone();
            var pnames = lambda.paramNames;
            var expr = lambda.expr_func;

            NonVoidDelegate dele = delegate(T param)
            {
                if (expr != null)
                {
                    try
                    {

                        content.DepthAdd();

                        content.DefineAndSet(pnames[0], typeof(T), param);
                        CLS_Content.Value retValue = expr.ComputeValue(content);

                        content.DepthRemove();

                        return (ReturnType)retValue.value;
                    }
                    catch (Exception err)
                    {
                        string errinfo = "Dump Call lambda in:";
                        if (content.CallType != null) errinfo += content.CallType.Name + "::";
                        if (content.function != null) errinfo += content.function;
                        errinfo += "\n";
                        env.logger.Log(errinfo + content.Dump());
                        throw err;
                    }
                }
                return default(ReturnType);
            };

            Delegate d = dele as Delegate;
            return Delegate.CreateDelegate(this.type, d.Target, d.Method);
        }
    }
}
