#import "common.typ": *

#show: js.with(
  lang: "ja",
  sansfont: "Harano Aji Gothic",//sansfont: "Microsoft Sans Serif", // or "Helvetica", "Source Sans Pro", "Arial", ...
  seriffont-cjk: "Harano Aji Mincho",
  sansfont-cjk: "Harano Aji Gothic",
)

#maketitle(
  title: "FlexID ユーザーマニュアル",
  authors: "HARA, Kenji",
  //abstract: [
  //  内部被ばく線量評価コードFlexID (Flexible code for Internal Dosimetry)の
  //  ユーザーマニュアル
  //],
)

#outline()
#pagebreak()

#include "UserManual_Main.typ"
#pagebreak()

= 添付資料

#pagebreak()
#set page(numbering: "A-1")
#include "UserManual_Appendix-1.typ"

#pagebreak()
#set page(numbering: "A-2")
#include "UserManual_Appendix-2.typ"

#pagebreak()
#set page(numbering: "A-3")
#include "UserManual_Appendix-3.typ"
