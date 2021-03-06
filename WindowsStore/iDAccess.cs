﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace App2
{
    // http://www.controlid.com.br/suporte/api_idaccess_V2.2-1.html#50_introduction
    // { login: 'admin', password: 'admin'}
    [DataContract]
    public class LoginRequest
    {
        [DataMember]
        public string login;
        [DataMember]
        public string password;
    }

    [DataContract]
    public class LoginResult
    {
        [DataMember]
        public string session;
        [DataMember]
        public string error;
    }

    // http://www.controlid.com.br/suporte/api_idaccess_V2.2-1.html#50_execute_actions
    // execute_actions.fcgi?session=" + session
    // actions: [ { action: "door", parameters: "door=1" } ]
    [DataContract()]
    public class ActionsRequest
    {
        [DataMember()]
        public ActionItem[] actions; // array de actionItem
    }

    [DataContract]
    public class ActionItem
    {
        [DataMember]
        public string action;
        [DataMember]
        public string parameters;
    }

}
