#import "@preview/js:0.1.3": *

#let maketitle(
  title: "",
  authors: "",
  abstract: [],
  keywords: (),
) = {
  set document(title: title, author: authortext(authors), keywords: keywords)
  place(top + center, scope: "parent", float: true)[
    #set align(center)
    #v(2em)
    #text(1.7em, title)
    #v(2em)
    #if authors != "" [
      #pad(
        x: 2em,
        if type(authors) == array {
          authors.map(boxtable).join("      ")
        } else {
          authors
        }
      )
    ]
    // #let date = datetime.today().display("[year]年[month repr:numerical padding:none]月[day padding:none]日"),
    // #if date != none [
    //   #v(1em)
    //   #date
    // ]
  ]
}

#let style(title: "", appendix: none, authors: "", body) = {
  show: js.with(
    seriffont:     "New Computer Modern",
    seriffont-cjk: "Harano Aji Mincho",
    sansfont:      "Harano Aji Gothic",
    sansfont-cjk:  "Harano Aji Gothic",
  )

  // ページの余白を設定
  set page(margin: (x: 25mm, top: 25mm, bottom: 15mm))

  // 図の下に空行を挿入する
  // #show figure: it => {
  //   if it.kind == image { [ #it #v(1em) ] } else { [ #it #v(1em) ] }
  // }

  // figureに入ったtableが複数ページにまたがることを許す
  show figure: set block(breakable: true)

  //show figure: it => {
  //  //if it.kind == image { [ #it #v(1em) ] } else { [ #v(1em) #it #v(1em) ] }
  //  [ #v(1em) #it #v(1em) ]
  //}

// 図表（figure）の上の余白を2em、下の余白を2emに設定
  //show figure: set block(above: 1em, below: 1em)

  //#show enum.item: it => {
  //  if it.body.has("children") { it } else { it }
  //}

  let page-prefix = ""
  if appendix != none {
    page-prefix = [付#appendix - ]
  }

  // 目次セクション
  counter(page).update(1)
  set page(numbering: (n, ..) => [#page-prefix #numbering("i", n)])
  outline()
  pagebreak()

  // 本文セクション
  counter(page).update(1)
  counter(heading).update(0)
  set page(numbering: (n, ..) => [#page-prefix #numbering("1", n)])
  set heading(numbering: "1.")
  set enum(numbering: "1)")
  set math.equation(numbering: "(1)")

  // 文書タイトル
  if appendix != none {
    maketitle(title: [添付資料#appendix 「#title」], authors: authors)
  } else {
    maketitle(title: title, authors: authors)
  }

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

#let markrect(image, factor: 100%, stroke: 2pt, color: red, ..rects) = {
  [
    #scale(factor, origin: top+left, reflow: true)[
      #image

      #for b in rects.pos() {
        let (x, y, w, h) = (b.at("x"), b.at("y"), b.at("w"), b.at("h"))
        let color = b.at("color", default: color)
        let stroke = b.at("stroke", default: stroke) / float(factor)
        place(
          top + left,
          dx: x, dy: y,
          rect(
            width: w, height: h,
            stroke: (paint: color, thickness: stroke),
            fill: none,
          ),
        )
      }
    ]
  ]
}
