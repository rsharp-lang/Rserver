// export R# package module type define for javascript/typescript language
//
//    imports "vbhtml" from "RwebHost";
//
// ref=RwebHost.VBHtmlTemplate@RwebHost, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace vbhtml {
   /**
   */
   function compile(file: string): any;
   /**
     * @param symbols default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function rendering(file: string, symbols?: object, env?: object): any;
}
