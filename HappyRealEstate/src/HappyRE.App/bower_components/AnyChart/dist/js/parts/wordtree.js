if(!_.wordtree){_.wordtree=1;(function($){var o4=function(){$.U.call(this);$.R(this.ua,ooa)},p4=function(a,b){$.mx.call(this);$.ku(this,this,this.zf,this.cg,this.ys,this.zf,null,null);this.Br=null;this.Du=[];this.qh=[];this.oO=this.yL=null;this.Pf=[];this.ft=[];$.R(this.ua,[["minFontSize",4,1],["maxFontSize",4,1],["postfix",4,1],["fontFamily",4,1],["fontStyle",16,1],["fontWeight",4,1],["fontColor",16,1],["fontOpacity",16,1],["fontDecoration",16,1]]);this.data(a,b)},poa=function(a,b){var c=b.getParent();if(c){for(var d=0;d<c.ac();d++){var e=
c.wd(d);e!=b&&e.o("hidden",!0)}poa(a,c)}},qoa=function(a){for(var b=0;b<a.qh.length;b++)a.qh[b].o("hidden",!1)},roa=function(a,b,c){if(c.length){for(var d=b.Ml(),e=!1,f=0;f<b.ac();f++)if(d[f].get("value")==c[0]){e=!0;break}e?(c.shift(),b=d[f]):(b=b.gc({value:c[0]}),c.shift());return roa(a,b,c)}return b},soa=function(a){a=a.split(/\s*(.+?(?:[?!]+|$|\.(?=\s+[A-Z]|$)))\s*/);return a=(0,$.Ye)(a,function(a){return!!a.length})},q4=function(a){a=a.split(/([!?,;:.&"-]+|\S*[A-Z]\.|\S*(?:[^!?,;:.\s&-]))/);
return a=(0,$.Ye)(a,function(a){return a.length&&" "!=a})},toa=function(a,b){null!=a.ka&&($.dr(a.ka,a.bd,a),a.ka.kd());a.ka=b;$.K(a.ka,a.bd,a);a.B(4100,1)},uoa=function(a,b,c){if(b){var d=b.get("fontSize");if(null==d){if((d=b.getParent())&&(1==d.ac()||c))return d=uoa(a,d,c),b.o("fontSize",d),d;c=a.g("maxFontSize");a=a.g("minFontSize");d=b.o("height")/1.5;d=$.Za(d,a,c);b.o("fontSize",d)}return d}return 0},voa=function(a,b){if(b){var c=b.ac(),d=0;if(c)for(var e=0;e<c;e++)d+=voa(a,b.wd(e));else d=b.get("weight")||
1;b.o("leafCount",d).o("weight",d);return d}return 0},woa=function(a,b,c,d,e,f){if(b){b.o("connectorInXPosition",c[0]).o("connectorInYPosition",c[1]).o("connectorOutXPosition",c[0]+a.QV(b)).o("connectorOutYPosition",c[1]);var h=a.fg.ab();if(d<h){c=a.ij();var k=b.ac(),l=c.g("length"),m=c.g("offset");c=c.g("curveFactor");var p=a.g("minFontSize");if(k){if(1<k){var q=p/10;e+=q;f-=q}q=e;for(var r=!1,t,u=0;u<k;u++)if(t=b.wd(u),t.o("hidden")){r=!0;break}if((f-e)/k<1.5*p&&1<k){k="+"+b.o("leafCount")+" "+
a.g("postfix");var v=(f+e)/2;m=d;d=a.Qx(m,v,k,b.o("fontSize")||p,a.g("fontFamily"),a.g("fontWeight"));v-=d.PG()/2;d.y(v);d.tag.node=b;d.tag.u2=!0;k=d.FT();m+k>=h&&d.visible(!1);a.Ox(b.o("connectorOutXPosition"),b.o("connectorOutYPosition"),b.o("connectorOutXPosition")+20,(f+e)/2,c)}else for(r&&(d-=l+m),u=0;u<k;u++)if(t=b.wd(u),!t.o("hidden")){m=t.o("leafCount");h=b.o("leafCount");h=r?f-e:(f-e)*Math.max(1,m)/Math.max(1,h);p=q;var w=0;q=p+h;m=d;v=p+h/2;var x=[m,v];t.o("height",h).o("nodePositionX",
m).o("nodePositionY",v);a.tL(t,r);v=b.o("textHeight")/2;x[1]-=v;1!=t.ac()&&(w+=l);m=a.QV(t);v=[x[0],x[1]+v];w+=x[0]+m;woa(a,t,v,w,p,p+h);1<b.ac()&&!r&&a.Ox(b.o("connectorOutXPosition"),b.o("connectorOutYPosition"),t.o("connectorInXPosition"),t.o("connectorInYPosition"),c)}}}}},xoa=function(a,b){var c=new p4(a,b);c.Fa("wordtree");return c};$.H(o4,$.U);var r4={};$.nq(r4,[[0,"curveFactor",$.Zq],[0,"offset",$.Wq],[0,"length",$.Wq],[1,"stroke",$.Jq]]);$.S(o4,r4);o4.prototype.oa=8201;
var ooa=[["curveFactor",0,8],["offset",0,8],["length",0,8],["stroke",0,8192]];o4.prototype.F=function(){var a=o4.u.F.call(this);$.Nq(this,r4,a);return a};o4.prototype.U=function(a,b){o4.u.U.call(this,a,b);$.Fq(this,r4,a,b)};$.H(p4,$.mx);p4.prototype.oa=$.mx.prototype.oa;p4.prototype.qa=$.mx.prototype.qa|4112;var s4=function(){var a={};$.nq(a,[[0,"minFontSize",$.Wq],[0,"maxFontSize",$.Wq],[0,"fontFamily",$.uq],[0,"fontStyle",$.$k],[0,"fontWeight",$.rq],[0,"fontColor",$.vq],[0,"fontOpacity",$.wq],[0,"fontDecoration",$.Zk],[0,"postfix",function(a){if($.n(a))return null===a&&(a=this.pf("postfix")),String(a)}]]);return a}();$.S(p4,s4);$.g=p4.prototype;$.g.Na=function(){return"wordtree"};
$.g.sj=function(){return!(this.ka&&(!this.ka||this.ka.ac()))};$.g.Qe=function(){return[]};$.g.ns=function(){return[this]};$.g.CU=function(a,b){return function(c){return a*(1-c)+b*c}};$.g.Ck=function(a,b){return $.iu(this.data(),b)};$.g.ij=function(a){this.Ej||(this.Ej=new o4,$.W(this,"connectors",this.Ej),$.K(this.Ej,this.vha,this));return a?(this.Ej.N(a),this):this.Ej};
$.g.Jx=function(a){this.Ii||(this.Ii=new $.xw);var b={};a&&(b.value={value:a.get("value"),type:"string"},b.weight={value:a.o("weight"),type:"number"});this.Ii.gg(a);return $.gv(this.Ii,b)};$.g.ar=function(a){if(a){"array"==$.ka(a)&&(a=a[0]);if(!$.J(a,$.Wt)&&(a=this.data().zp("value",a)[0],!a))return;this.yL=a;poa(this,a);this.B(20,1)}};$.g.Rx=function(){qoa(this);this.yL&&this.yL.getParent()&&this.ar(this.yL.getParent());return this};
$.g.vha=function(a){var b=0;$.X(a,8192)&&(b|=16);$.X(a,8)&&(b|=4);this.B(b,1)};$.g.bd=function(a){$.X(a,16)&&this.B(4100,9)};$.g.ys=function(a){a.button==$.Oi&&(a=a.domTarget.tag)&&a.node&&(qoa(this),this.ar(a.node))};$.g.zf=function(a){var b=a.domTarget.tag;if(b&&b.node&&!b.u2){var c=b.node;b=this.Ra();$.Gw(b,a.clientX,a.clientY,this.Jx(c))}else this.Ra().Ic()};$.g.cg=function(){this.Ra().Ic()};
$.g.WX=function(a){if("implicit"==this.qe){if($.n(a)){if(this.oO!=a||this.J0){this.J0=!1;if(null===a||/^[\s\xa0]*$/.test(a))a=this.dw[0][0];this.oO=a;for(var b=[],c=0;c<this.dw.length;c++){var d=(0,$.za)(this.dw[c],a);-1!=d&&b.push($.Ja(this.dw[c],d))}b.length||(b[0]=[a]);a=b[0][0];$.V(this.data());this.data().ac()&&this.data().vj(0);c={value:a};c=this.data().Ag(c,0);for(d=0;d<b.length;d++){var e=b[d];e[0]==a&&(e.shift(),roa(this,c,e))}this.data().ea(!0)}return this}return this.oO}return this};
$.g.data=function(a,b){if($.n(a)){if($.J(a,$.Tt)||$.J(a,$.Qt))this.ka!=a&&(this.ka&&$.dr(this.ka,this.bd,this),this.ka=a,$.K(this.ka,this.bd,this),this.qe="explicit",this.B(4100,1));else if("array"==$.ka(a)&&"object"==$.ka(a[0]))this.data($.gu(a,b));else if(null===a)this.ka&&($.dr(this.ka,this.bd),this.ka.kd()),this.ka=null,this.B(4100,1);else{this.qe="implicit";this.Pf=a;if("array"==$.ka(a)&&a.length)if(this.dw=[],"array"==$.ka(a[0]))for(var c=0;c<a.length;c++)this.dw.push(q4(a[c][0]));else{if("string"==
$.ka(a[0]))for(c=0;c<a.length;c++)this.dw.push(q4(a[c]))}else if("string"==$.ka(a)){c=soa(a);for(var d=[],e=0;e<c.length;e++)d.push(q4(c[e]));this.dw=d}else this.dw=[[a.toString()]];this.ka||toa(this,$.gu());this.J0=!0;this.WX(this.dw[0][0])}return this}return this.ka};$.g.Yh=function(){var a;0<this.ft.length?a=this.ft.pop():a=new $.Oh;return a};$.g.QV=function(a){if(a){var b=a.o("textWidth"),c=this.ij().g("offset"),d=a.getParent();d&&1<d.ac()&&(b+=c);1<a.ac()&&(b+=c);return b}return 0};
$.g.depth=function(a){if(a){var b=a.ac(),c=0,d=a.getParent();d&&(c=d.o("depth")+1);a.o("depth",c);for(c=0;c<b;c++)this.depth(a.wd(c))}};$.g.Qx=function(a,b,c,d,e,f){var h=this.Yh();h.tag||(h.tag={});h.text(c);h.y(b);h.x(a);h.fontSize(d);h.fontFamily(e);h.fontWeight(f);h.width(null);h.visible(!0);this.Du.push(h);return h};$.g.Ox=function(a,b,c,d,e){var f=this.qC,h=this.CU(a,c),k=h(e);e=h(1-e);f.moveTo(a,b).Kk(k,b,e,d,c,d)};
$.g.tL=function(a,b){var c=0,d=a.getParent();d&&1<d.ac()&&(c+=this.ij().g("offset"));var e=a.get("value");1==a.ac()&&(e+=" ");c=a.o("nodePositionX")+c;d=a.o("nodePositionY");var f=a.get("fontFamily")||this.g("fontFamily");var h=a.get("fontWeight")||this.g("fontWeight");var k=uoa(this,a,b);f=this.Qx(c,d,e,k,f,h);f.tag={node:a,type:"node",u2:!1};h=f.PG();e=f.FT();a.o("textHeight",h).o("textWidth",e);d-=h/2;h=c+e;k=this.fg.ab();h>k&&f.width(e-(h-k)-.05*e);f.x(c).y(d)};
$.g.Kh=function(a){if(!this.kf())if(this.xa||(this.xa=this.Ma.Ad(),this.qC=new $.xg,this.xa.zIndex(30)),this.sj())this.xa.Wi();else{var b=this.Br=this.ka.wd(0);this.J(4096)&&(voa(this,this.Br),this.depth(this.Br),this.qh.length=0,this.qh=this.data().Sv().ZB(),this.I(4096));if(this.J(4)){this.xa.Wi();var c=a.left;var d=a.top;a=a.height;for(var e=this.ij(),f=e.g("offset"),h=0;h<this.Du.length;h++){var k=this.Du[h];this.ft.push(k)}this.Du.length=0;this.qC.clear();this.xa.suspend();k=[c+f,d+a/2];b.o("nodePositionX",
k[0]).o("nodePositionY",k[1]).o("height",a);this.tL(b,!1);c=c+f+this.QV(b)+(1<b.ac()?e.g("length"):0);woa(this,b,k,c,d,d+a);this.qC.parent(this.xa);this.I(4);for(h=0;h<this.Du.length;h++)this.Du[h].parent(this.xa);this.xa.resume();this.B(16)}if(this.J(16)){this.xa.suspend();this.qC.stroke(this.ij().g("stroke"));d=this.g("fontColor");c=this.g("fontDecoration");a=this.g("fontStyle");e=this.g("fontOpacity");for(f=0;f<this.Du.length;f++){b=this.Du[f];if(b.tag&&b.tag.node){var l=b.tag.node;var m=l.get("fontColor")||
d;var p=l.get("fontDecoration")||c;var q=l.get("fontStyle")||a;l=l.get("fontOpacity")||e}b.color(m);b.Xq(p);b.fontStyle(q);b.opacity(l);b.Am(!1)}this.xa.resume();this.I(16)}}};$.g.F=function(){var a=p4.u.F.call(this);$.Nq(this,s4,a);"implicit"==this.qe&&(a.wordTreeRawData=JSON.stringify(this.Pf),a.word=this.oO);a.treeData=this.data().cB(["hidden"]);a.connectors=this.ij().F();return{chart:a}};
$.g.U=function(a,b){p4.u.U.call(this,a,b);$.Fq(this,s4,a,b);"wordTreeRawData"in a?(this.data(JSON.parse(a.wordTreeRawData)),"word"in a&&this.WX(a.word),"treeData"in a&&toa(this,$.Vt(a.treeData))):"treeData"in a&&this.data($.Vt(a.treeData));"connectors"in a&&this.ij().fa(!!b,a.connectors)};$.g.R=function(){p4.u.R.call(this);$.sd(this.Ej,this.qC,this.Du,this.ft,this.ka,this.xa);this.ka=this.qC=this.Ej=null;this.Du.length=0;this.ft.length=0;this.xa=null};var t4=p4.prototype;t4.connectors=t4.ij;
t4.word=t4.WX;t4.getType=t4.Na;t4.drillTo=t4.ar;t4.drillUp=t4.Rx;t4.toCsv=t4.Ck;t4.noData=t4.zm;$.Op.wordtree=xoa;$.F("anychart.wordtree",xoa);}).call(this,$)}