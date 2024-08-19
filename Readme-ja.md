[English](./Readme.md)

# _FlexID (Flexible code for Internal Dosimetry)_

**FlexID**は、**最新のICRP**（国際放射線防護委員会）**内部被ばく線量評価モデル**（放射性核種の職業上の摂取(OIR: Occupational Intakes of Radionuclides)）に対応した**計算コード**である。

ICRP2007年勧告に基づく体内動態モデルを臓器・組織毎に組合せ、放射性核種の子孫核種も含めた人間の体内での移行を計算し、体内残留放射能、臓器・組織における積算放射能、尿中・糞中の排泄放射能、等価線量率、及び等価線量の時系列データ、並びに預託実効線量を計算する。

入力データの変更だけで体内動態モデル（多コンパートメントモデル）の組合せ、接続パス、及び移行係数等を変更でき、体内動態モデルの変更に迅速に対応可能な汎用コードである。
例えば、ICRPで適用されている標準人のモデルに対して、日本人固有の甲状腺へのヨウ素の移行係数の適用も入力データの変更だけで容易に対応できる。

なお、FlexIDの妥当性確認として以下の核種について、FlexIDとICRP(ICRP Publication134,137,141)の計算結果を比較し、それらが一致することを確認した。

## 妥当性確認(OIR:職業人)

- **対象核種**：
    H-3, C-14, Ca-45, Fe-59, Zn-65, Sr-90(Y-90), Tc-99, I-129, Ba-133, Cs-134, Cs-137(Ba-137m), Ra-223, Ra-226, Pu-238, Pu-239, Pu-240, Pu-241, Pu-242 (括弧内は子孫核種)

- **摂取経路**：
    経口摂取, 吸入摂取

- **化学形態**：
    元素ごとにICRPで体内動態モデルが定義されている全ての化学形態

- **確認対象とした計算結果**：
    残留放射能, 排泄放射能, 預託実効線量換算係数

## 妥当性確認(公衆:3ヵ月, 1, 5, 10, 15, 25歳(※1))

また、試験的に現行ICRPの「公衆」に対する計算機能をFlexIDに実装し、Sr-90の経口摂取における計算結果の妥当性を確認した。その結果、FlexIDとICRPの計算結果が概ね一致することを確認した。

- **対象核種(※2)**：
Sr-90(Y-90)

- **摂取経路**：
経口摂取

- **確認対象とした計算結果**：
    残留放射能, 排泄放射能, 預託実効線量換算係数


**(※1)** : 殆どの元素では成人を20歳としているが、アルカリ土類元素、鉛、トリウム、ウラン、ネプツニウム、プルトニウム、アメリシウム、及びキュリウムは例外で成人を25歳としている。これは、体内動態モデルの移行係数の一部が骨の形成速度と同等であり、これらの元素では約25歳まで移行係数が上昇すると推定されるためである。

**(※2)** : ICRP Publ.30(Part 1)：全元素共通の胃腸管モデル(核種移行係数含む) / ICRP Publ.38：核崩壊データ(年齢別の比実効エネルギーSEE(Specific Effective Energy; OIRにおけるS係数)計算に使用) / ICRP Publ.60：放射線加重係数(SEE計算に使用) / ORNL/TM8381/V1-V7：年齢別の比吸収割合(SAF: Specific Absorbed Fraction) / ICRP Publ.67：ストロンチウムとイットリウムの組織系動態モデル(年齢別の核種移行係数含む)、f1値(年齢別の胃腸管から体液への吸収割合) / ICRP Publ.71：年齢別の膀胱からの排泄割合、組織加重係数(SEE計算に使用)、年齢別の臓器・組織の質量(SEE計算に使用)

## プログラム構成

### _FlexID_

以下のプログラムによって構成される。

- **FlexID**

  FlexIDの入力GUIを提供する。

- **FlexID.Calc**

  FlexIDの計算処理を実行する。

- **FlexID.Viewer**

  FlexIDの計算結果を可視化する。

## ドキュメント

[ユーザーマニュアル (日本語)](./docs/UserManual_jp.pdf).

## 動作要件

.NET Framework 4.6.2

## 動作画面

### _**入力GUI**_

![img](./docs/images/input.jpg)

### _**プロット図**_  ( Cs-137(Type F: 速い吸収) 1 [Bq] 吸入摂取後の残留放射能 )

![img](./docs/images/plot.jpg)

### _**コンターアニメーション**_  ( Cs-137(Type F: 速い吸収) 1 [Bq] 吸入摂取後の等価線量 )

![img](./docs/images/animation.jpg)

## 参考文献

### **職業人(OIR)**
- [ICRP Publication 130](https://icrp.org/publication.asp?id=ICRP%20Publication%20130)
- [ICRP Publication 133](https://www.icrp.org/publication.asp?id=ICRP%20Publication%20133)
- [ICRP Publication 134](https://icrp.org/publication.asp?id=ICRP%20Publication%20134)
- [ICRP Publication 137](https://icrp.org/publication.asp?id=ICRP%20Publication%20137)
- [ICRP Publication 141](https://icrp.org/publication.asp?id=ICRP%20Publication%20141)
- [ICRP Publication 151](https://icrp.org/publication.asp?id=ICRP%20Publication%20151)

### **公衆**
- [ICRP Publication 30 (Part 1)](https://icrp.org/publication.asp?id=ICRP%20Publication%2030%20(Part%201))
- [ICRP Publication 38](https://icrp.org/publication.asp?id=ICRP%20Publication%2038)
- [ICRP Publication 56](https://icrp.org/publication.asp?id=ICRP%20Publication%2056)
- [ICRP Publication 60](https://icrp.org/publication.asp?id=ICRP%20Publication%2060)
- [ICRP Publication 67](https://icrp.org/publication.asp?id=ICRP%20Publication%2067)
- [ICRP Publication 69](https://icrp.org/publication.asp?id=ICRP%20Publication%2069)
- [ICRP Publication 71](https://icrp.org/publication.asp?id=ICRP%20Publication%2071)
- [ICRP Publication 72](https://icrp.org/publication.asp?id=ICRP%20Publication%2072)

## ライセンス

Copyright © MHI NSエンジニアリング株式会社

**FlexID**は **MIT license** に基づいて現状のまま提供されます。詳細については、[LICENSE](./LICENSE)を参照してください。
