﻿/*
 * Copyright (c) 2021 Akitsugu Komiyama
 * under the MIT License
 */


using System;
using System.Collections.Generic;



// ★秀丸クラス
public sealed partial class hmV8DynamicLib
{
    public sealed partial class Hidemaru
    {
        public sealed class OutputPane
        {
            static OutputPane()
            {
                SetUnManagedDll();
            }


            // Output枠へと出力する
            public static int Output(object message)
            {
                bool isClearScriptItem = false;
                try
                {
                    if (message.GetType().Name == "V8ScriptItem" || message.GetType().BaseType?.Name == "V8ScriptItem" || message.GetType().BaseType?.BaseType?.Name == "V8ScriptItem")
                    {
                        isClearScriptItem = true;
                    }
                }
                catch (Exception)
                {

                }

                string str_message = "";
                // V8エンジンのオブジェクトであれば、そのまま出しても意味が無いので…
                if (isClearScriptItem)
                {
                    dynamic dexp = message;

                    // JSON.stringifyで変換できるはずだ
                    try
                    {
                        str_message = engine.Script.JSON.stringify(dexp, engine.Script._stringify_replacer, 2);
                        if (str_message != null)
                        {
                            str_message = str_message.Replace("\r\n", "\n").Replace("\n", "\r\n");
                        }
                    }
                    catch (Exception)
                    {

                    }

                    // JSON.stringfyで空っぽだったようだ。
                    if (str_message == String.Empty)
                    {
                        try
                        {
                            // ECMAScriptのtoString()で変換出来るはずだ…
                            str_message = dexp.toString();
                        }
                        catch (Exception)
                        {
                            // 変換できなかったら、とりあえず、しょうがないので.NETのToStringで。多分意味はない。
                            str_message = message.ToString();
                        }
                    }
                }

                // V8オブジェクトでないなら、普通にToString
                else
                {
                    str_message = message.ToString();
                }

                try
                {
                    if (pOutputPane_OutputW != null)
                    {
                        int result = pOutputPane_OutputW(Hidemaru.WindowHandle, str_message);
                        return result;
                    }
                    else
                    {
                        byte[] encode_data = HmOriginalEncodeFunc.EncodeWStringToOriginalEncodeVector(str_message);
                        int result = pOutputPane_Output(Hidemaru.WindowHandle, encode_data);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    OutputDebugStream(ErrorMsg.MethodNeedOutputOperation + ":\n" + e.Message);
                }

                return 0;
            }

            // Output枠でのPush
            public static int Push()
            {
                return pOutputPane_Push(Hidemaru.WindowHandle); ;
            }

            // Output枠でのPush
            public static int Pop()
            {
                return pOutputPane_Pop(Hidemaru.WindowHandle); ;
            }

            public static int Clear()
            {
                //1009=クリア
                IntPtr r = OutputPane.SendMessage(1009);
                int ret = (int)HmClamp<long>((long)r, Int32.MinValue, Int32.MaxValue);
                return ret;
            }

            public static IntPtr WindowHandle
            {
                get
                {
                    return pGetWinndowHandle(Hidemaru.WindowHandle);
                }
            }

            public static IntPtr SendMessage(int commandID)
            {
                //
                // loaddll "HmOutputPane.dll";
                // #h=dllfunc("GetWindowHandle",hidemaruhandle(0));
                // #ret=sendmessage(#h,0x111,1009,0);//1009=クリア 0x111=WM_COMMAND
                //
                IntPtr result = hmV8DynamicLib.SendMessage(OutputPane.WindowHandle, 0x111, commandID, IntPtr.Zero);
                return result;
            }

            // Output枠へと出力する
            public static int SetBaseDir(string dirpath)
            {
                if (version < 877)
                {
                    OutputDebugStream(ErrorMsg.MethodNeed877);
                    return 0;
                }

                try
                {
                    if (pOutputPane_SetBaseDir != null)
                    {
                        byte[] encode_data = HmOriginalEncodeFunc.EncodeWStringToOriginalEncodeVector(dirpath);
                        int result = pOutputPane_SetBaseDir(Hidemaru.WindowHandle, encode_data);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    OutputDebugStream(ErrorMsg.MethodNeedOutputOperation + ":\n" + e.Message);
                }

                return 0;
            }

        }
    }
}

