#import "@preview/js:0.1.3": *

#show: js.with(
  lang: "ja",
  sansfont: "Harano Aji Gothic",//sansfont: "Microsoft Sans Serif", // or "Helvetica", "Source Sans Pro", "Arial", ...
  seriffont-cjk: "Harano Aji Mincho",
  sansfont-cjk: "Harano Aji Gothic",
)

// ページの余白を設定
#set page(margin: (x: 20mm, y: 20mm))

// 図の下に空行を挿入する
#show figure: it => {
  if it.kind == image { [ #it #v(1em) ] } else { it }
}

// ラベルから「番号 タイトル」のリンクを作成する
#let chapref(label) = context {
  let h = query(label).first()
  let num = numbering(
    h.numbering,
    ..counter(heading).at(h.location())
  )
  link(h.location())[ 「#num #h.body」 ]
}

//#set enum(numbering: "①")

//#show enum.item: it => {
//  if it.body.has("children") { it } else { it }
//}

#maketitle(
  title: "添付資料1 「体内残留放射能の計算方法」",
  authors: "HARA, Kenji",
  //abstract: [
  //  内部被ばく線量評価コードFlexID (Flexible code for Internal Dosimetry)の
  //  ユーザーマニュアル
  //],
)

#outline()
#pagebreak()

= FlexIDの体内残留放射能の計算方法

放射性物質摂取（経口摂取／吸入摂取）時の各臓器／器官／組織の放射性物質の放射能と被ばく線量をICRP OIRシリーズの体内動態モデルに基づき算出する内部被ばく線量評価コードFlexIDの計算方法を示す。

== 残留放射能計算方法の概要

1. 各臓器等を機能毎に分類し、その分類に基づき計算機能（入力（摂取：仮想ノードである摂取ノードにのみ設定可能）、混合、蓄積）を作成。その計算機能を組み合わせることで体内挙動モデルを形成する。

2. 計算は時間を分割し、先ず親核種について各臓器等の放射能が収束するまで繰り返し計算を行う。これを時間分割について行う。次に子孫核種について順次同様の収束計算を実施する。

    #figure(
      image("images/Figure_A1-1.png"),
      caption: "",
    )

    臓器1，2は計算機能1を、臓器3は計算機能2を使用する。

    臓器3から臓器2へ入力があるため、収束計算を行う。

    機能：混合、蓄積（流入、流出、蓄積）、排出

3. 各計算機能は次図のように入力と出力に分けて取り扱う。子孫核種も同様に取り扱う。（子孫核種への移行も臓器等の間での移行として取り扱う。）

    #figure(
      image("images/Figure_A1-2.png"),
      caption: "",
    )

1. 核種により体内での挙動が異なる場合の取扱

 親核種と子孫核種で挙動が異なる場合（例えば、Te-131⇒I-131）は、核種の分類によりその流出先を指定することで対応する。


== 基本計算式

以下の方法で体内の時系列放射能分布を計算する。

計算方法にはノード・ジャンクション法を採用し、臓器等の間での接続パス（ジャンクション）に設定された移行係数と移行割合から、各臓器等（ノード）の放射能を時間メッシュ毎に計算する。各時間メッシュにおいては、全てのノードの放射能が収束するまで繰り返し計算を実施する。

また、ノードは蓄積機能と混合機能のどちらかを選択でき、混合機能では複数の流入に対して平均値を計算し、その後、流出ジャンクションから移行割合に応じて流出させる。蓄積機能については、以下の基本式によりノード内の放射能の蓄積を計算する。

- 蓄積機能における前提条件

    #figure(
      image("images/Figure_A1-3.png"),
      caption: "",
    )

    流入放射能（$A$ \[Bq/d\]）は時間メッシュ期間中に一定であると仮定する。

- 放射能$N$の計算

  $frac(d N, d t) = A - (lambda + beta) dot.c N $

  $N = A dot.c frac(1 - exp(-(lambda + beta) dot.c t), lambda + beta) +
        N_0 dot.c exp(-(lambda + beta) dot.c t) $
 
  ここで、

  #table(
    columns: 2,
    [式], [意味],
    [$lambda$], [崩壊定数 \[/d\]],
    [$beta$],   [流出放射能 \[Bq/d\]],
    [$N_0$],    [当該時間メッシュにおける初期放射能 \[Bq\]],
  )

- 時間積分放射能$Q$の計算

// $$\begin{align*}
//     Q &= \int\limits_0^T N_i\ \mathrm{d}t \\
//       &= N_0 \cdot            \frac{1 - \exp{\{ -(\lambda + \beta) \cdot T \}}}{\lambda + \beta}
//        + A   \cdot \frac{ T - \frac{1 - \exp{\{ -(\lambda + \beta) \cdot T \}}}{\lambda + \beta} }{\lambda + \beta}
//   \end{align*}$$

- 時間平均放射能$overline(N)$の計算
