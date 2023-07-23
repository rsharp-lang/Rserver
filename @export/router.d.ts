// export R# package module type define for javascript/typescript language
//
//    imports "router" from "RwebHost";
//
// ref=RwebHost.RouterFunction@RwebHost, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace router {
   /**
     * @param env default value Is ``null``.
   */
   function handle(req: object, response: object, router: object, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function parse(exp: object, env?: object): any;
}
