#import "@preview/js:0.1.3": *

#let style(body) = {
  show: js

  // ページの余白を設定
  set page(margin: (x: 25mm, y: 25mm))

  // 図の下に空行を挿入する
  // #show figure: it => {
  //   if it.kind == image { [ #it #v(1em) ] } else { [ #it #v(1em) ] }
  // }

  // figureに入ったtableが複数ページにまたがることを許す
  show figure: set block(breakable: true)

  show figure: it => {
    if it.kind == image { [ #it #v(1em) ] } else { [ #it #v(1em) ] }
  }

  //#show enum.item: it => {
  //  if it.body.has("children") { it } else { it }
  //}

  body
}

// ラベルから「番号 タイトル」のリンクを作成する
#let chapref(label) = context {
  let matches = query(label)

  if matches.len() == 0 {
    text(red)[[UNKNOWN: #label]]
  } else {
    let h = matches.first()
    let num = numbering(
      h.numbering,
      ..counter(heading).at(h.location())
    )
    box[「#link(h.location())[#box[#num #h.body]]」]
  }
}
