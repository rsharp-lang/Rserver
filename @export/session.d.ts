// export R# package module type define for javascript/typescript language
//
//    imports "session" from "RwebHost";
//
// ref=RwebHost.Session@RwebHost, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace session {
   /**
   */
   function get_integer(key: string): object;
   /**
   */
   function get_number(key: string): number;
   /**
   */
   function get_string(key: string): string;
   /**
     * @param env default value Is ``null``.
   */
   function load(env?: object): string;
   /**
   */
   function session_id(): string;
   /**
   */
   function set_string(key: string, value: string): any;
}
