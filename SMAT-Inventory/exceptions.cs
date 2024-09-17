using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SMAT_Inventory.Exceptions
{
    public class DATABASE_NOT_FOUND_EXCEPTION : SystemException
    {
        string _message = "Database not found.";

        public DATABASE_NOT_FOUND_EXCEPTION()
        {

        }

        public DATABASE_NOT_FOUND_EXCEPTION(string Message)
            : base(Message)
        {
            _message = Message;
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    public class DUPLICATE_EMAIL_EXCEPTION : SystemException
    {
        string _message = "Email already exists.";

        public DUPLICATE_EMAIL_EXCEPTION()
        {

        }

        public DUPLICATE_EMAIL_EXCEPTION(string Message)
            : base(Message)
        {
            _message = Message;
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    public class DUPLICATE_USER_NAME_EXCEPTION : SystemException
    {
        string _message = "User Name already exists.";

        public DUPLICATE_USER_NAME_EXCEPTION()
        {

        }

        public DUPLICATE_USER_NAME_EXCEPTION(string Message)
            : base(Message)
        {
            _message = Message;
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    public class INVALID_USER_NAME_EXCEPTION : SystemException
    {
        string _message = "User Name does not exist.";

        public INVALID_USER_NAME_EXCEPTION()
        {

        }

        public INVALID_USER_NAME_EXCEPTION(string Message)
            : base(Message)
        {
            _message = Message;
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    public class LOGIN_FAILED_EXCEPTION : SystemException
    {
        string _message = "SQL Server login failed.";

        public LOGIN_FAILED_EXCEPTION()
        {

        }

        public LOGIN_FAILED_EXCEPTION(string Message)
            : base(Message)
        {
            _message = Message;
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    public class SERVER_NOT_FOUND_EXCEPTION : SystemException
    {
        string _message = "SQL Server specified does not exist.";

        public SERVER_NOT_FOUND_EXCEPTION()
        {

        }

        public SERVER_NOT_FOUND_EXCEPTION(string Message)
            : base(Message)
        {
            _message = Message;
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    public class SERVER_CONNECTION_EXCEPTION : SystemException
    {
        string _message = "Could not connect to SQL Server, either server is offline or network related issue occured.";

        public SERVER_CONNECTION_EXCEPTION()
        {

        }

        public SERVER_CONNECTION_EXCEPTION(string Message)
            : base(Message)
        {
            _message = Message;
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }

    public class UNIQUE_KEY_CONSTRAINT_EXCEPTION : SystemException
    {
        string _message = "Unique key value constraint failed.";

        public UNIQUE_KEY_CONSTRAINT_EXCEPTION()
        {

        }

        public UNIQUE_KEY_CONSTRAINT_EXCEPTION(string Message)
            : base(Message)
        {
            _message = Message;
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }
}