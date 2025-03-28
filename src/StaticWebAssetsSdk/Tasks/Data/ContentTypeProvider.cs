// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.AspNetCore.StaticWebAssets.Tasks;

internal sealed class ContentTypeProvider
{
    private static readonly Dictionary<string, ContentTypeMapping> _builtInMappings =
        new()
        {
            ["*.js"] = new ContentTypeMapping("text/javascript", null, "*.js", 1),
            ["*.css"] = new ContentTypeMapping("text/css", null, "*.css", 1),
            ["*.html"] = new ContentTypeMapping("text/html", null, "*.html", 1),
            ["*.json"] = new ContentTypeMapping("application/json", null, "*.json", 1),
            ["*.mjs"] = new ContentTypeMapping("text/javascript", null, "*.mjs", 1),
            ["*.xml"] = new ContentTypeMapping("text/xml", null, "*.xml", 1),
            ["*.htm"] = new ContentTypeMapping("text/html", null, "*.htm", 1),
            ["*.wasm"] = new ContentTypeMapping("application/wasm", null, "*.wasm", 1),
            ["*.txt"] = new ContentTypeMapping("text/plain", null, "*.txt", 1),
            ["*.dll"] = new ContentTypeMapping("application/octet-stream", null, "*.dll", 1),
            ["*.pdb"] = new ContentTypeMapping("application/octet-stream", null, "*.pdb", 1),
            ["*.dat"] = new ContentTypeMapping("application/octet-stream", null, "*.dat", 1),
            ["*.webmanifest"] = new ContentTypeMapping("application/manifest+json", null, "*.webmanifest", 1),
            ["*.jsx"] = new ContentTypeMapping("text/jscript", null, "*.jsx", 1),
            ["*.markdown"] = new ContentTypeMapping("text/markdown", null, "*.markdown", 1),
            ["*.gz"] = new ContentTypeMapping("application/x-gzip", null, "*.gz", 1),
            ["*.br"] = new ContentTypeMapping("application/octet-stream", null, "*.br", 1),
            ["*.md"] = new ContentTypeMapping("text/markdown", null, "*.md", 1),
            ["*.bmp"] = new ContentTypeMapping("image/bmp", null, "*.bmp", 1),
            ["*.jpeg"] = new ContentTypeMapping("image/jpeg", null, "*.jpeg", 1),
            ["*.jpg"] = new ContentTypeMapping("image/jpeg", null, "*.jpg", 1),
            ["*.gif"] = new ContentTypeMapping("image/gif", null, "*.gif", 1),
            ["*.svg"] = new ContentTypeMapping("image/svg+xml", null, "*.svg", 1),
            ["*.png"] = new ContentTypeMapping("image/png", null, "*.png", 1),
            ["*.webp"] = new ContentTypeMapping("image/webp", null, "*.webp", 1),
            ["*.otf"] = new ContentTypeMapping("font/otf", null, "*.otf", 1),
            ["*.woff2"] = new ContentTypeMapping("font/woff2", null, "*.woff2", 1),
            ["*.m4v"] = new ContentTypeMapping("video/mp4", null, "*.m4v", 1),
            ["*.mov"] = new ContentTypeMapping("video/quicktime", null, "*.mov", 1),
            ["*.movie"] = new ContentTypeMapping("video/x-sgi-movie", null, "*.movie", 1),
            ["*.mp2"] = new ContentTypeMapping("video/mpeg", null, "*.mp2", 1),
            ["*.mp4"] = new ContentTypeMapping("video/mp4", null, "*.mp4", 1),
            ["*.mp4v"] = new ContentTypeMapping("video/mp4", null, "*.mp4v", 1),
            ["*.mpa"] = new ContentTypeMapping("video/mpeg", null, "*.mpa", 1),
            ["*.mpe"] = new ContentTypeMapping("video/mpeg", null, "*.mpe", 1),
            ["*.mpeg"] = new ContentTypeMapping("video/mpeg", null, "*.mpeg", 1),
            ["*.mpg"] = new ContentTypeMapping("video/mpeg", null, "*.mpg", 1),
            ["*.mpv2"] = new ContentTypeMapping("video/mpeg", null, "*.mpv2", 1),
            ["*.nsc"] = new ContentTypeMapping("video/x-ms-asf", null, "*.nsc", 1),
            ["*.ogg"] = new ContentTypeMapping("video/ogg", null, "*.ogg", 1),
            ["*.ogv"] = new ContentTypeMapping("video/ogg", null, "*.ogv", 1),
            ["*.webm"] = new ContentTypeMapping("video/webm", null, "*.webm", 1),
            ["*.323"] = new ContentTypeMapping("text/h323", null, "*.323", 1),
            ["*.appcache"] = new ContentTypeMapping("text/cache-manifest", null, "*.appcache", 1),
            ["*.asm"] = new ContentTypeMapping("text/plain", null, "*.asm", 1),
            ["*.bas"] = new ContentTypeMapping("text/plain", null, "*.bas", 1),
            ["*.c"] = new ContentTypeMapping("text/plain", null, "*.c", 1),
            ["*.cnf"] = new ContentTypeMapping("text/plain", null, "*.cnf", 1),
            ["*.cpp"] = new ContentTypeMapping("text/plain", null, "*.cpp", 1),
            ["*.csv"] = new ContentTypeMapping("text/csv", null, "*.csv", 1),
            ["*.disco"] = new ContentTypeMapping("text/xml", null, "*.disco", 1),
            ["*.dlm"] = new ContentTypeMapping("text/dlm", null, "*.dlm", 1),
            ["*.dtd"] = new ContentTypeMapping("text/xml", null, "*.dtd", 1),
            ["*.etx"] = new ContentTypeMapping("text/x-setext", null, "*.etx", 1),
            ["*.h"] = new ContentTypeMapping("text/plain", null, "*.h", 1),
            ["*.hdml"] = new ContentTypeMapping("text/x-hdml", null, "*.hdml", 1),
            ["*.htc"] = new ContentTypeMapping("text/x-component", null, "*.htc", 1),
            ["*.htt"] = new ContentTypeMapping("text/webviewhtml", null, "*.htt", 1),
            ["*.hxt"] = new ContentTypeMapping("text/html", null, "*.hxt", 1),
            ["*.ical"] = new ContentTypeMapping("text/calendar", null, "*.ical", 1),
            ["*.icalendar"] = new ContentTypeMapping("text/calendar", null, "*.icalendar", 1),
            ["*.ics"] = new ContentTypeMapping("text/calendar", null, "*.ics", 1),
            ["*.ifb"] = new ContentTypeMapping("text/calendar", null, "*.ifb", 1),
            ["*.map"] = new ContentTypeMapping("text/plain", null, "*.map", 1),
            ["*.mno"] = new ContentTypeMapping("text/xml", null, "*.mno", 1),
            ["*.odc"] = new ContentTypeMapping("text/x-ms-odc", null, "*.odc", 1),
            ["*.rtx"] = new ContentTypeMapping("text/richtext", null, "*.rtx", 1),
            ["*.sct"] = new ContentTypeMapping("text/scriptlet", null, "*.sct", 1),
            ["*.sgml"] = new ContentTypeMapping("text/sgml", null, "*.sgml", 1),
            ["*.tsv"] = new ContentTypeMapping("text/tab-separated-values", null, "*.tsv", 1),
            ["*.uls"] = new ContentTypeMapping("text/iuls", null, "*.uls", 1),
            ["*.vbs"] = new ContentTypeMapping("text/vbscript", null, "*.vbs", 1),
            ["*.vcf"] = new ContentTypeMapping("text/x-vcard", null, "*.vcf", 1),
            ["*.vcs"] = new ContentTypeMapping("text/plain", null, "*.vcs", 1),
            ["*.vml"] = new ContentTypeMapping("text/xml", null, "*.vml", 1),
            ["*.wml"] = new ContentTypeMapping("text/vnd.wap.wml", null, "*.wml", 1),
            ["*.wmls"] = new ContentTypeMapping("text/vnd.wap.wmlscript", null, "*.wmls", 1),
            ["*.wsdl"] = new ContentTypeMapping("text/xml", null, "*.wsdl", 1),
            ["*.xdr"] = new ContentTypeMapping("text/plain", null, "*.xdr", 1),
            ["*.xsd"] = new ContentTypeMapping("text/xml", null, "*.xsd", 1),
            ["*.xsf"] = new ContentTypeMapping("text/xml", null, "*.xsf", 1),
            ["*.xsl"] = new ContentTypeMapping("text/xml", null, "*.xsl", 1),
            ["*.xslt"] = new ContentTypeMapping("text/xml", null, "*.xslt", 1),
            ["*.woff"] = new ContentTypeMapping("application/font-woff", null, "*.woff", 1),
            ["*.art"] = new ContentTypeMapping("image/x-jg", null, "*.art", 1),
            ["*.cmx"] = new ContentTypeMapping("image/x-cmx", null, "*.cmx", 1),
            ["*.cod"] = new ContentTypeMapping("image/cis-cod", null, "*.cod", 1),
            ["*.dib"] = new ContentTypeMapping("image/bmp", null, "*.dib", 1),
            ["*.ico"] = new ContentTypeMapping("image/x-icon", null, "*.ico", 1),
            ["*.ief"] = new ContentTypeMapping("image/ief", null, "*.ief", 1),
            ["*.jfif"] = new ContentTypeMapping("image/pjpeg", null, "*.jfif", 1),
            ["*.jpe"] = new ContentTypeMapping("image/jpeg", null, "*.jpe", 1),
            ["*.pbm"] = new ContentTypeMapping("image/x-portable-bitmap", null, "*.pbm", 1),
            ["*.pgm"] = new ContentTypeMapping("image/x-portable-graymap", null, "*.pgm", 1),
            ["*.pnm"] = new ContentTypeMapping("image/x-portable-anymap", null, "*.pnm", 1),
            ["*.pnz"] = new ContentTypeMapping("image/png", null, "*.pnz", 1),
            ["*.ppm"] = new ContentTypeMapping("image/x-portable-pixmap", null, "*.ppm", 1),
            ["*.ras"] = new ContentTypeMapping("image/x-cmu-raster", null, "*.ras", 1),
            ["*.rf"] = new ContentTypeMapping("image/vnd.rn-realflash", null, "*.rf", 1),
            ["*.rgb"] = new ContentTypeMapping("image/x-rgb", null, "*.rgb", 1),
            ["*.svgz"] = new ContentTypeMapping("image/svg+xml", null, "*.svgz", 1),
            ["*.tif"] = new ContentTypeMapping("image/tiff", null, "*.tif", 1),
            ["*.tiff"] = new ContentTypeMapping("image/tiff", null, "*.tiff", 1),
            ["*.wbmp"] = new ContentTypeMapping("image/vnd.wap.wbmp", null, "*.wbmp", 1),
            ["*.xbm"] = new ContentTypeMapping("image/x-xbitmap", null, "*.xbm", 1),
            ["*.xpm"] = new ContentTypeMapping("image/x-xpixmap", null, "*.xpm", 1),
            ["*.xwd"] = new ContentTypeMapping("image/x-xwindowdump", null, "*.xwd", 1),
            ["*.3g2"] = new ContentTypeMapping("video/3gpp2", null, "*.3g2", 1),
            ["*.3gp2"] = new ContentTypeMapping("video/3gpp2", null, "*.3gp2", 1),
            ["*.3gp"] = new ContentTypeMapping("video/3gpp", null, "*.3gp", 1),
            ["*.3gpp"] = new ContentTypeMapping("video/3gpp", null, "*.3gpp", 1),
            ["*.asf"] = new ContentTypeMapping("video/x-ms-asf", null, "*.asf", 1),
            ["*.asr"] = new ContentTypeMapping("video/x-ms-asf", null, "*.asr", 1),
            ["*.asx"] = new ContentTypeMapping("video/x-ms-asf", null, "*.asx", 1),
            ["*.avi"] = new ContentTypeMapping("video/x-msvideo", null, "*.avi", 1),
            ["*.dvr"] = new ContentTypeMapping("video/x-ms-dvr", null, "*.dvr", 1),
            ["*.flv"] = new ContentTypeMapping("video/x-flv", null, "*.flv", 1),
            ["*.IVF"] = new ContentTypeMapping("video/x-ivf", null, "*.IVF", 1),
            ["*.lsf"] = new ContentTypeMapping("video/x-la-asf", null, "*.lsf", 1),
            ["*.lsx"] = new ContentTypeMapping("video/x-la-asf", null, "*.lsx", 1),
            ["*.m1v"] = new ContentTypeMapping("video/mpeg", null, "*.m1v", 1),
            ["*.m2ts"] = new ContentTypeMapping("video/vnd.dlna.mpeg-tts", null, "*.m2ts", 1),
            ["*.qt"] = new ContentTypeMapping("video/quicktime", null, "*.qt", 1),
            ["*.ts"] = new ContentTypeMapping("video/vnd.dlna.mpeg-tts", null, "*.ts", 1),
            ["*.tts"] = new ContentTypeMapping("video/vnd.dlna.mpeg-tts", null, "*.tts", 1),
            ["*.wm"] = new ContentTypeMapping("video/x-ms-wm", null, "*.wm", 1),
            ["*.wmp"] = new ContentTypeMapping("video/x-ms-wmp", null, "*.wmp", 1),
            ["*.wmv"] = new ContentTypeMapping("video/x-ms-wmv", null, "*.wmv", 1),
            ["*.wmx"] = new ContentTypeMapping("video/x-ms-wmx", null, "*.wmx", 1),
            ["*.wtv"] = new ContentTypeMapping("video/x-ms-wtv", null, "*.wtv", 1),
            ["*.wvx"] = new ContentTypeMapping("video/x-ms-wvx", null, "*.wvx", 1),
            ["*.aac"] = new ContentTypeMapping("audio/aac", null, "*.aac", 1),
            ["*.adt"] = new ContentTypeMapping("audio/vnd.dlna.adts", null, "*.adt", 1),
            ["*.adts"] = new ContentTypeMapping("audio/vnd.dlna.adts", null, "*.adts", 1),
            ["*.aif"] = new ContentTypeMapping("audio/x-aiff", null, "*.aif", 1),
            ["*.aifc"] = new ContentTypeMapping("audio/aiff", null, "*.aifc", 1),
            ["*.aiff"] = new ContentTypeMapping("audio/aiff", null, "*.aiff", 1),
            ["*.au"] = new ContentTypeMapping("audio/basic", null, "*.au", 1),
            ["*.m3u"] = new ContentTypeMapping("audio/x-mpegurl", null, "*.m3u", 1),
            ["*.m4a"] = new ContentTypeMapping("audio/mp4", null, "*.m4a", 1),
            ["*.mid"] = new ContentTypeMapping("audio/mid", null, "*.mid", 1),
            ["*.midi"] = new ContentTypeMapping("audio/mid", null, "*.midi", 1),
            ["*.mp3"] = new ContentTypeMapping("audio/mpeg", null, "*.mp3", 1),
            ["*.oga"] = new ContentTypeMapping("audio/ogg", null, "*.oga", 1),
            ["*.ra"] = new ContentTypeMapping("audio/x-pn-realaudio", null, "*.ra", 1),
            ["*.ram"] = new ContentTypeMapping("audio/x-pn-realaudio", null, "*.ram", 1),
            ["*.rmi"] = new ContentTypeMapping("audio/mid", null, "*.rmi", 1),
            ["*.rpm"] = new ContentTypeMapping("audio/x-pn-realaudio-plugin", null, "*.rpm", 1),
            ["*.smd"] = new ContentTypeMapping("audio/x-smd", null, "*.smd", 1),
            ["*.smx"] = new ContentTypeMapping("audio/x-smd", null, "*.smx", 1),
            ["*.smz"] = new ContentTypeMapping("audio/x-smd", null, "*.smz", 1),
            ["*.snd"] = new ContentTypeMapping("audio/basic", null, "*.snd", 1),
            ["*.spx"] = new ContentTypeMapping("audio/ogg", null, "*.spx", 1),
            ["*.wav"] = new ContentTypeMapping("audio/wav", null, "*.wav", 1),
            ["*.wax"] = new ContentTypeMapping("audio/x-ms-wax", null, "*.wax", 1),
            ["*.wma"] = new ContentTypeMapping("audio/x-ms-wma", null, "*.wma", 1),
            ["*.accdb"] = new ContentTypeMapping("application/msaccess", null, "*.accdb", 1),
            ["*.accde"] = new ContentTypeMapping("application/msaccess", null, "*.accde", 1),
            ["*.accdt"] = new ContentTypeMapping("application/msaccess", null, "*.accdt", 1),
            ["*.acx"] = new ContentTypeMapping("application/internet-property-stream", null, "*.acx", 1),
            ["*.ai"] = new ContentTypeMapping("application/postscript", null, "*.ai", 1),
            ["*.application"] = new ContentTypeMapping("application/x-ms-application", null, "*.application", 1),
            ["*.atom"] = new ContentTypeMapping("application/atom+xml", null, "*.atom", 1),
            ["*.axs"] = new ContentTypeMapping("application/olescript", null, "*.axs", 1),
            ["*.bcpio"] = new ContentTypeMapping("application/x-bcpio", null, "*.bcpio", 1),
            ["*.cab"] = new ContentTypeMapping("application/vnd.ms-cab-compressed", null, "*.cab", 1),
            ["*.calx"] = new ContentTypeMapping("application/vnd.ms-office.calx", null, "*.calx", 1),
            ["*.cat"] = new ContentTypeMapping("application/vnd.ms-pki.seccat", null, "*.cat", 1),
            ["*.cdf"] = new ContentTypeMapping("application/x-cdf", null, "*.cdf", 1),
            ["*.class"] = new ContentTypeMapping("application/x-java-applet", null, "*.class", 1),
            ["*.clp"] = new ContentTypeMapping("application/x-msclip", null, "*.clp", 1),
            ["*.cpio"] = new ContentTypeMapping("application/x-cpio", null, "*.cpio", 1),
            ["*.crd"] = new ContentTypeMapping("application/x-mscardfile", null, "*.crd", 1),
            ["*.crl"] = new ContentTypeMapping("application/pkix-crl", null, "*.crl", 1),
            ["*.crt"] = new ContentTypeMapping("application/x-x509-ca-cert", null, "*.crt", 1),
            ["*.csh"] = new ContentTypeMapping("application/x-csh", null, "*.csh", 1),
            ["*.dcr"] = new ContentTypeMapping("application/x-director", null, "*.dcr", 1),
            ["*.der"] = new ContentTypeMapping("application/x-x509-ca-cert", null, "*.der", 1),
            ["*.dir"] = new ContentTypeMapping("application/x-director", null, "*.dir", 1),
            ["*.doc"] = new ContentTypeMapping("application/msword", null, "*.doc", 1),
            ["*.docm"] = new ContentTypeMapping("application/vnd.ms-word.document.macroEnabled.12", null, "*.docm", 1),
            ["*.docx"] = new ContentTypeMapping("application/vnd.openxmlformats-officedocument.wordprocessingml.document", null, "*.docx", 1),
            ["*.dot"] = new ContentTypeMapping("application/msword", null, "*.dot", 1),
            ["*.dotm"] = new ContentTypeMapping("application/vnd.ms-word.template.macroEnabled.12", null, "*.dotm", 1),
            ["*.dotx"] = new ContentTypeMapping("application/vnd.openxmlformats-officedocument.wordprocessingml.template", null, "*.dotx", 1),
            ["*.dvi"] = new ContentTypeMapping("application/x-dvi", null, "*.dvi", 1),
            ["*.dwf"] = new ContentTypeMapping("drawing/x-dwf", null, "*.dwf", 1),
            ["*.dxr"] = new ContentTypeMapping("application/x-director", null, "*.dxr", 1),
            ["*.eml"] = new ContentTypeMapping("message/rfc822", null, "*.eml", 1),
            ["*.eot"] = new ContentTypeMapping("application/vnd.ms-fontobject", null, "*.eot", 1),
            ["*.eps"] = new ContentTypeMapping("application/postscript", null, "*.eps", 1),
            ["*.evy"] = new ContentTypeMapping("application/envoy", null, "*.evy", 1),
            ["*.exe"] = new ContentTypeMapping("application/vnd.microsoft.portable-executable", null, "*.exe", 1),
            ["*.fdf"] = new ContentTypeMapping("application/vnd.fdf", null, "*.fdf", 1),
            ["*.fif"] = new ContentTypeMapping("application/fractals", null, "*.fif", 1),
            ["*.flr"] = new ContentTypeMapping("x-world/x-vrml", null, "*.flr", 1),
            ["*.gtar"] = new ContentTypeMapping("application/x-gtar", null, "*.gtar", 1),
            ["*.hdf"] = new ContentTypeMapping("application/x-hdf", null, "*.hdf", 1),
            ["*.hhc"] = new ContentTypeMapping("application/x-oleobject", null, "*.hhc", 1),
            ["*.hlp"] = new ContentTypeMapping("application/winhlp", null, "*.hlp", 1),
            ["*.hqx"] = new ContentTypeMapping("application/mac-binhex40", null, "*.hqx", 1),
            ["*.hta"] = new ContentTypeMapping("application/hta", null, "*.hta", 1),
            ["*.iii"] = new ContentTypeMapping("application/x-iphone", null, "*.iii", 1),
            ["*.ins"] = new ContentTypeMapping("application/x-internet-signup", null, "*.ins", 1),
            ["*.isp"] = new ContentTypeMapping("application/x-internet-signup", null, "*.isp", 1),
            ["*.jar"] = new ContentTypeMapping("application/java-archive", null, "*.jar", 1),
            ["*.jck"] = new ContentTypeMapping("application/liquidmotion", null, "*.jck", 1),
            ["*.jcz"] = new ContentTypeMapping("application/liquidmotion", null, "*.jcz", 1),
            ["*.latex"] = new ContentTypeMapping("application/x-latex", null, "*.latex", 1),
            ["*.lit"] = new ContentTypeMapping("application/x-ms-reader", null, "*.lit", 1),
            ["*.m13"] = new ContentTypeMapping("application/x-msmediaview", null, "*.m13", 1),
            ["*.m14"] = new ContentTypeMapping("application/x-msmediaview", null, "*.m14", 1),
            ["*.man"] = new ContentTypeMapping("application/x-troff-man", null, "*.man", 1),
            ["*.manifest"] = new ContentTypeMapping("application/x-ms-manifest", null, "*.manifest", 1),
            ["*.mdb"] = new ContentTypeMapping("application/x-msaccess", null, "*.mdb", 1),
            ["*.me"] = new ContentTypeMapping("application/x-troff-me", null, "*.me", 1),
            ["*.mht"] = new ContentTypeMapping("message/rfc822", null, "*.mht", 1),
            ["*.mhtml"] = new ContentTypeMapping("message/rfc822", null, "*.mhtml", 1),
            ["*.mmf"] = new ContentTypeMapping("application/x-smaf", null, "*.mmf", 1),
            ["*.mny"] = new ContentTypeMapping("application/x-msmoney", null, "*.mny", 1),
            ["*.mpp"] = new ContentTypeMapping("application/vnd.ms-project", null, "*.mpp", 1),
            ["*.ms"] = new ContentTypeMapping("application/x-troff-ms", null, "*.ms", 1),
            ["*.mvb"] = new ContentTypeMapping("application/x-msmediaview", null, "*.mvb", 1),
            ["*.mvc"] = new ContentTypeMapping("application/x-miva-compiled", null, "*.mvc", 1),
            ["*.nc"] = new ContentTypeMapping("application/x-netcdf", null, "*.nc", 1),
            ["*.nws"] = new ContentTypeMapping("message/rfc822", null, "*.nws", 1),
            ["*.oda"] = new ContentTypeMapping("application/oda", null, "*.oda", 1),
            ["*.ods"] = new ContentTypeMapping("application/oleobject", null, "*.ods", 1),
            ["*.ogx"] = new ContentTypeMapping("application/ogg", null, "*.ogx", 1),
            ["*.one"] = new ContentTypeMapping("application/onenote", null, "*.one", 1),
            ["*.onea"] = new ContentTypeMapping("application/onenote", null, "*.onea", 1),
            ["*.onetoc"] = new ContentTypeMapping("application/onenote", null, "*.onetoc", 1),
            ["*.onetoc2"] = new ContentTypeMapping("application/onenote", null, "*.onetoc2", 1),
            ["*.onetmp"] = new ContentTypeMapping("application/onenote", null, "*.onetmp", 1),
            ["*.onepkg"] = new ContentTypeMapping("application/onenote", null, "*.onepkg", 1),
            ["*.osdx"] = new ContentTypeMapping("application/opensearchdescription+xml", null, "*.osdx", 1),
            ["*.p10"] = new ContentTypeMapping("application/pkcs10", null, "*.p10", 1),
            ["*.p12"] = new ContentTypeMapping("application/x-pkcs12", null, "*.p12", 1),
            ["*.p7b"] = new ContentTypeMapping("application/x-pkcs7-certificates", null, "*.p7b", 1),
            ["*.p7c"] = new ContentTypeMapping("application/pkcs7-mime", null, "*.p7c", 1),
            ["*.p7m"] = new ContentTypeMapping("application/pkcs7-mime", null, "*.p7m", 1),
            ["*.p7r"] = new ContentTypeMapping("application/x-pkcs7-certreqresp", null, "*.p7r", 1),
            ["*.p7s"] = new ContentTypeMapping("application/pkcs7-signature", null, "*.p7s", 1),
            ["*.pdf"] = new ContentTypeMapping("application/pdf", null, "*.pdf", 1),
            ["*.pfx"] = new ContentTypeMapping("application/x-pkcs12", null, "*.pfx", 1),
            ["*.pko"] = new ContentTypeMapping("application/vnd.ms-pki.pko", null, "*.pko", 1),
            ["*.pma"] = new ContentTypeMapping("application/x-perfmon", null, "*.pma", 1),
            ["*.pmc"] = new ContentTypeMapping("application/x-perfmon", null, "*.pmc", 1),
            ["*.pml"] = new ContentTypeMapping("application/x-perfmon", null, "*.pml", 1),
            ["*.pmr"] = new ContentTypeMapping("application/x-perfmon", null, "*.pmr", 1),
            ["*.pmw"] = new ContentTypeMapping("application/x-perfmon", null, "*.pmw", 1),
            ["*.pot"] = new ContentTypeMapping("application/vnd.ms-powerpoint", null, "*.pot", 1),
            ["*.potm"] = new ContentTypeMapping("application/vnd.ms-powerpoint.template.macroEnabled.12", null, "*.potm", 1),
            ["*.potx"] = new ContentTypeMapping("application/vnd.openxmlformats-officedocument.presentationml.template", null, "*.potx", 1),
            ["*.ppam"] = new ContentTypeMapping("application/vnd.ms-powerpoint.addin.macroEnabled.12", null, "*.ppam", 1),
            ["*.pps"] = new ContentTypeMapping("application/vnd.ms-powerpoint", null, "*.pps", 1),
            ["*.ppsm"] = new ContentTypeMapping("application/vnd.ms-powerpoint.slideshow.macroEnabled.12", null, "*.ppsm", 1),
            ["*.ppsx"] = new ContentTypeMapping("application/vnd.openxmlformats-officedocument.presentationml.slideshow", null, "*.ppsx", 1),
            ["*.ppt"] = new ContentTypeMapping("application/vnd.ms-powerpoint", null, "*.ppt", 1),
            ["*.pptm"] = new ContentTypeMapping("application/vnd.ms-powerpoint.presentation.macroEnabled.12", null, "*.pptm", 1),
            ["*.pptx"] = new ContentTypeMapping("application/vnd.openxmlformats-officedocument.presentationml.presentation", null, "*.pptx", 1),
            ["*.prf"] = new ContentTypeMapping("application/pics-rules", null, "*.prf", 1),
            ["*.ps"] = new ContentTypeMapping("application/postscript", null, "*.ps", 1),
            ["*.pub"] = new ContentTypeMapping("application/x-mspublisher", null, "*.pub", 1),
            ["*.qtl"] = new ContentTypeMapping("application/x-quicktimeplayer", null, "*.qtl", 1),
            ["*.rm"] = new ContentTypeMapping("application/vnd.rn-realmedia", null, "*.rm", 1),
            ["*.roff"] = new ContentTypeMapping("application/x-troff", null, "*.roff", 1),
            ["*.rtf"] = new ContentTypeMapping("application/rtf", null, "*.rtf", 1),
            ["*.scd"] = new ContentTypeMapping("application/x-msschedule", null, "*.scd", 1),
            ["*.setpay"] = new ContentTypeMapping("application/set-payment-initiation", null, "*.setpay", 1),
            ["*.setreg"] = new ContentTypeMapping("application/set-registration-initiation", null, "*.setreg", 1),
            ["*.sh"] = new ContentTypeMapping("application/x-sh", null, "*.sh", 1),
            ["*.shar"] = new ContentTypeMapping("application/x-shar", null, "*.shar", 1),
            ["*.sit"] = new ContentTypeMapping("application/x-stuffit", null, "*.sit", 1),
            ["*.sldm"] = new ContentTypeMapping("application/vnd.ms-powerpoint.slide.macroEnabled.12", null, "*.sldm", 1),
            ["*.sldx"] = new ContentTypeMapping("application/vnd.openxmlformats-officedocument.presentationml.slide", null, "*.sldx", 1),
            ["*.spc"] = new ContentTypeMapping("application/x-pkcs7-certificates", null, "*.spc", 1),
            ["*.spl"] = new ContentTypeMapping("application/futuresplash", null, "*.spl", 1),
            ["*.src"] = new ContentTypeMapping("application/x-wais-source", null, "*.src", 1),
            ["*.ssm"] = new ContentTypeMapping("application/streamingmedia", null, "*.ssm", 1),
            ["*.sst"] = new ContentTypeMapping("application/vnd.ms-pki.certstore", null, "*.sst", 1),
            ["*.stl"] = new ContentTypeMapping("application/vnd.ms-pki.stl", null, "*.stl", 1),
            ["*.sv4cpio"] = new ContentTypeMapping("application/x-sv4cpio", null, "*.sv4cpio", 1),
            ["*.sv4crc"] = new ContentTypeMapping("application/x-sv4crc", null, "*.sv4crc", 1),
            ["*.swf"] = new ContentTypeMapping("application/x-shockwave-flash", null, "*.swf", 1),
            ["*.t"] = new ContentTypeMapping("application/x-troff", null, "*.t", 1),
            ["*.tar"] = new ContentTypeMapping("application/x-tar", null, "*.tar", 1),
            ["*.tcl"] = new ContentTypeMapping("application/x-tcl", null, "*.tcl", 1),
            ["*.tex"] = new ContentTypeMapping("application/x-tex", null, "*.tex", 1),
            ["*.texi"] = new ContentTypeMapping("application/x-texinfo", null, "*.texi", 1),
            ["*.texinfo"] = new ContentTypeMapping("application/x-texinfo", null, "*.texinfo", 1),
            ["*.tgz"] = new ContentTypeMapping("application/x-compressed", null, "*.tgz", 1),
            ["*.thmx"] = new ContentTypeMapping("application/vnd.ms-officetheme", null, "*.thmx", 1),
            ["*.tr"] = new ContentTypeMapping("application/x-troff", null, "*.tr", 1),
            ["*.trm"] = new ContentTypeMapping("application/x-msterminal", null, "*.trm", 1),
            ["*.ttc"] = new ContentTypeMapping("application/x-font-ttf", null, "*.ttc", 1),
            ["*.ttf"] = new ContentTypeMapping("application/x-font-ttf", null, "*.ttf", 1),
            ["*.ustar"] = new ContentTypeMapping("application/x-ustar", null, "*.ustar", 1),
            ["*.vdx"] = new ContentTypeMapping("application/vnd.ms-visio.viewer", null, "*.vdx", 1),
            ["*.vsd"] = new ContentTypeMapping("application/vnd.visio", null, "*.vsd", 1),
            ["*.vss"] = new ContentTypeMapping("application/vnd.visio", null, "*.vss", 1),
            ["*.vst"] = new ContentTypeMapping("application/vnd.visio", null, "*.vst", 1),
            ["*.vsto"] = new ContentTypeMapping("application/x-ms-vsto", null, "*.vsto", 1),
            ["*.vsw"] = new ContentTypeMapping("application/vnd.visio", null, "*.vsw", 1),
            ["*.vsx"] = new ContentTypeMapping("application/vnd.visio", null, "*.vsx", 1),
            ["*.vtx"] = new ContentTypeMapping("application/vnd.visio", null, "*.vtx", 1),
            ["*.wcm"] = new ContentTypeMapping("application/vnd.ms-works", null, "*.wcm", 1),
            ["*.wdb"] = new ContentTypeMapping("application/vnd.ms-works", null, "*.wdb", 1),
            ["*.wks"] = new ContentTypeMapping("application/vnd.ms-works", null, "*.wks", 1),
            ["*.wmd"] = new ContentTypeMapping("application/x-ms-wmd", null, "*.wmd", 1),
            ["*.wmf"] = new ContentTypeMapping("application/x-msmetafile", null, "*.wmf", 1),
            ["*.wmlc"] = new ContentTypeMapping("application/vnd.wap.wmlc", null, "*.wmlc", 1),
            ["*.wmlsc"] = new ContentTypeMapping("application/vnd.wap.wmlscriptc", null, "*.wmlsc", 1),
            ["*.wmz"] = new ContentTypeMapping("application/x-ms-wmz", null, "*.wmz", 1),
            ["*.wps"] = new ContentTypeMapping("application/vnd.ms-works", null, "*.wps", 1),
            ["*.wri"] = new ContentTypeMapping("application/x-mswrite", null, "*.wri", 1),
            ["*.wrl"] = new ContentTypeMapping("x-world/x-vrml", null, "*.wrl", 1),
            ["*.wrz"] = new ContentTypeMapping("x-world/x-vrml", null, "*.wrz", 1),
            ["*.x"] = new ContentTypeMapping("application/directx", null, "*.x", 1),
            ["*.xaf"] = new ContentTypeMapping("x-world/x-vrml", null, "*.xaf", 1),
            ["*.xaml"] = new ContentTypeMapping("application/xaml+xml", null, "*.xaml", 1),
            ["*.xap"] = new ContentTypeMapping("application/x-silverlight-app", null, "*.xap", 1),
            ["*.xbap"] = new ContentTypeMapping("application/x-ms-xbap", null, "*.xbap", 1),
            ["*.xht"] = new ContentTypeMapping("application/xhtml+xml", null, "*.xht", 1),
            ["*.xhtml"] = new ContentTypeMapping("application/xhtml+xml", null, "*.xhtml", 1),
            ["*.xla"] = new ContentTypeMapping("application/vnd.ms-excel", null, "*.xla", 1),
            ["*.xlam"] = new ContentTypeMapping("application/vnd.ms-excel.addin.macroEnabled.12", null, "*.xlam", 1),
            ["*.xlc"] = new ContentTypeMapping("application/vnd.ms-excel", null, "*.xlc", 1),
            ["*.xlm"] = new ContentTypeMapping("application/vnd.ms-excel", null, "*.xlm", 1),
            ["*.xls"] = new ContentTypeMapping("application/vnd.ms-excel", null, "*.xls", 1),
            ["*.xlsb"] = new ContentTypeMapping("application/vnd.ms-excel.sheet.binary.macroEnabled.12", null, "*.xlsb", 1),
            ["*.xlsm"] = new ContentTypeMapping("application/vnd.ms-excel.sheet.macroEnabled.12", null, "*.xlsm", 1),
            ["*.xlsx"] = new ContentTypeMapping("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", null, "*.xlsx", 1),
            ["*.xlt"] = new ContentTypeMapping("application/vnd.ms-excel", null, "*.xlt", 1),
            ["*.xltm"] = new ContentTypeMapping("application/vnd.ms-excel.template.macroEnabled.12", null, "*.xltm", 1),
            ["*.xltx"] = new ContentTypeMapping("application/vnd.openxmlformats-officedocument.spreadsheetml.template", null, "*.xltx", 1),
            ["*.xlw"] = new ContentTypeMapping("application/vnd.ms-excel", null, "*.xlw", 1),
            ["*.xof"] = new ContentTypeMapping("x-world/x-vrml", null, "*.xof", 1),
            ["*.xps"] = new ContentTypeMapping("application/vnd.ms-xpsdocument", null, "*.xps", 1),
            ["*.z"] = new ContentTypeMapping("application/x-compress", null, "*.z", 1),
            ["*.zip"] = new ContentTypeMapping("application/x-zip-compressed", null, "*.zip", 1),
            ["*.aaf"] = new ContentTypeMapping("application/octet-stream", null, "*.aaf", 1),
            ["*.aca"] = new ContentTypeMapping("application/octet-stream", null, "*.aca", 1),
            ["*.afm"] = new ContentTypeMapping("application/octet-stream", null, "*.afm", 1),
            ["*.asd"] = new ContentTypeMapping("application/octet-stream", null, "*.asd", 1),
            ["*.asi"] = new ContentTypeMapping("application/octet-stream", null, "*.asi", 1),
            ["*.bin"] = new ContentTypeMapping("application/octet-stream", null, "*.bin", 1),
            ["*.chm"] = new ContentTypeMapping("application/octet-stream", null, "*.chm", 1),
            ["*.cur"] = new ContentTypeMapping("application/octet-stream", null, "*.cur", 1),
            ["*.deploy"] = new ContentTypeMapping("application/octet-stream", null, "*.deploy", 1),
            ["*.dsp"] = new ContentTypeMapping("application/octet-stream", null, "*.dsp", 1),
            ["*.dwp"] = new ContentTypeMapping("application/octet-stream", null, "*.dwp", 1),
            ["*.emz"] = new ContentTypeMapping("application/octet-stream", null, "*.emz", 1),
            ["*.fla"] = new ContentTypeMapping("application/octet-stream", null, "*.fla", 1),
            ["*.hhk"] = new ContentTypeMapping("application/octet-stream", null, "*.hhk", 1),
            ["*.hhp"] = new ContentTypeMapping("application/octet-stream", null, "*.hhp", 1),
            ["*.inf"] = new ContentTypeMapping("application/octet-stream", null, "*.inf", 1),
            ["*.java"] = new ContentTypeMapping("application/octet-stream", null, "*.java", 1),
            ["*.jpb"] = new ContentTypeMapping("application/octet-stream", null, "*.jpb", 1),
            ["*.lpk"] = new ContentTypeMapping("application/octet-stream", null, "*.lpk", 1),
            ["*.lzh"] = new ContentTypeMapping("application/octet-stream", null, "*.lzh", 1),
            ["*.mdp"] = new ContentTypeMapping("application/octet-stream", null, "*.mdp", 1),
            ["*.mix"] = new ContentTypeMapping("application/octet-stream", null, "*.mix", 1),
            ["*.msi"] = new ContentTypeMapping("application/octet-stream", null, "*.msi", 1),
            ["*.mso"] = new ContentTypeMapping("application/octet-stream", null, "*.mso", 1),
            ["*.ocx"] = new ContentTypeMapping("application/octet-stream", null, "*.ocx", 1),
            ["*.pcx"] = new ContentTypeMapping("application/octet-stream", null, "*.pcx", 1),
            ["*.pcz"] = new ContentTypeMapping("application/octet-stream", null, "*.pcz", 1),
            ["*.pfb"] = new ContentTypeMapping("application/octet-stream", null, "*.pfb", 1),
            ["*.pfm"] = new ContentTypeMapping("application/octet-stream", null, "*.pfm", 1),
            ["*.prm"] = new ContentTypeMapping("application/octet-stream", null, "*.prm", 1),
            ["*.prx"] = new ContentTypeMapping("application/octet-stream", null, "*.prx", 1),
            ["*.psd"] = new ContentTypeMapping("application/octet-stream", null, "*.psd", 1),
            ["*.psm"] = new ContentTypeMapping("application/octet-stream", null, "*.psm", 1),
            ["*.psp"] = new ContentTypeMapping("application/octet-stream", null, "*.psp", 1),
            ["*.qxd"] = new ContentTypeMapping("application/octet-stream", null, "*.qxd", 1),
            ["*.rar"] = new ContentTypeMapping("application/octet-stream", null, "*.rar", 1),
            ["*.sea"] = new ContentTypeMapping("application/octet-stream", null, "*.sea", 1),
            ["*.smi"] = new ContentTypeMapping("application/octet-stream", null, "*.smi", 1),
            ["*.snp"] = new ContentTypeMapping("application/octet-stream", null, "*.snp", 1),
            ["*.thn"] = new ContentTypeMapping("application/octet-stream", null, "*.thn", 1),
            ["*.toc"] = new ContentTypeMapping("application/octet-stream", null, "*.toc", 1),
            ["*.u32"] = new ContentTypeMapping("application/octet-stream", null, "*.u32", 1),
            ["*.xsn"] = new ContentTypeMapping("application/octet-stream", null, "*.xsn", 1),
            ["*.xtp"] = new ContentTypeMapping("application/octet-stream", null, "*.xtp", 1),
        };

    private readonly StaticWebAssetGlobMatcher _matcher;

    private readonly Dictionary<string, ContentTypeMapping> _customMappings;

    public ContentTypeProvider(ContentTypeMapping[] customMappings)
    {
        _customMappings ??= [];
        foreach (var mapping in customMappings)
        {
            _customMappings[mapping.Pattern] = mapping;
        }

        _matcher = new StaticWebAssetGlobMatcherBuilder()
            .AddIncludePatternsList(_builtInMappings.Keys)
            .AddIncludePatternsList(_customMappings.Keys)
            .Build();
    }

    // First we strip any compressed extension (e.g. .gz, .br) from the file name
    // and then we try to match the file name with the existing mappings.
    // If we don't find a match, we fallback to trying the entire file name.
    internal ContentTypeMapping ResolveContentTypeMapping(StaticWebAssetGlobMatcher.MatchContext context, TaskLoggingHelper log)
    {
#if NET9_0_OR_GREATER
        var relativePath = context.Path;
        var fileNameSpan = Path.GetFileName(context.Path);
        var fileName = relativePath[(relativePath.Length - fileNameSpan.Length)..];
#else
        var relativePath = context.PathString;
        var fileName = Path.GetFileName(relativePath);
#endif
        var fileNameNoCompressionExt = ResolvePathWithoutCompressedExtension(fileName, out var hasCompressedExtension);

        context.SetPathAndReinitialize(fileNameNoCompressionExt);
        if (TryGetMapping(context, log, relativePath, out var mapping))
        {
            return mapping;
        }
        else if (hasCompressedExtension)
        {
            context.SetPathAndReinitialize(fileName);
            if (hasCompressedExtension && TryGetMapping(context, log, relativePath, out mapping))
            {
                return mapping;
            }
        }

        return default;
    }

#if NET9_0_OR_GREATER
    private bool TryGetMapping(StaticWebAssetGlobMatcher.MatchContext context, TaskLoggingHelper log, ReadOnlySpan<char> relativePath, out ContentTypeMapping mapping)
#else
    private bool TryGetMapping(StaticWebAssetGlobMatcher.MatchContext context, TaskLoggingHelper log, string relativePath, out ContentTypeMapping mapping)
#endif
    {
        var match = _matcher.Match(context);
        if (match.IsMatch)
        {
            if (_builtInMappings.TryGetValue(match.Pattern, out mapping) || _customMappings.TryGetValue(match.Pattern, out mapping))
            {
                log.LogMessage(MessageImportance.Low, $"Matched {relativePath} to {mapping.MimeType} using pattern {match.Pattern}");
                return true;
            }
            else
            {
                throw new InvalidOperationException("Matched pattern but no mapping found.");
            }
        }

        mapping = default;
        return false;
    }

#if NET9_0_OR_GREATER
    private static ReadOnlySpan<char> ResolvePathWithoutCompressedExtension(ReadOnlySpan<char> fileName, out bool hasCompressedExtension)
#else
    private static string ResolvePathWithoutCompressedExtension(string fileName, out bool hasCompressedExtension)
#endif
    {
        var extension = Path.GetExtension(fileName);
        hasCompressedExtension = extension.Equals(".gz", StringComparison.OrdinalIgnoreCase) || extension.Equals(".br", StringComparison.OrdinalIgnoreCase);
        if (hasCompressedExtension)
        {
            var fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
            if (!Path.GetExtension(fileNameNoExtension).Equals("", StringComparison.Ordinal))
            {
#if NET9_0_OR_GREATER
                return fileName[..fileNameNoExtension.Length];
#else
                return fileName.Substring(0, fileNameNoExtension.Length);
#endif
            }
        }

        return fileName;
    }
}
