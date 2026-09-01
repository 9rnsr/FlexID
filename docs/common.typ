#import "@preview/js:0.1.3": *

// ページの余白を設定
#set page(margin: (x: 20mm, y: 20mm))

// 図の下に空行を挿入する
#show figure: it => {
  if it.kind == image { [ #it #v(1em) ] } else { [ #it #v(1em) ] }
}
#show figure: set block(breakable: true)

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
    link(h.location())[ 「#num #h.body」 ]
  }
}
