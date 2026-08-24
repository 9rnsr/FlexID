[{Co}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   RedMarrow             R-marrow      # only for progeny of Fe
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Spleen                Spleen        # only for progeny of Fe
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Co}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.146 Para.298
# > (298) Cobalt produced in tissue compartments in the model for iron is assumed to be
# > transferred to the central blood compartment in the cobalt model (identified with
# > other plasma in the iron model) with the following half-times: 1 min for RBCs and
# > plasma transferrin, 2 d for compartments of the liver, 30 d for spleen and compartments
# > of red marrow, and 7 d for other compartments. ...
# > (298) 鉄のモデルにおける組織コンパートメント内で生成されるコバルトは、コバルトのモデル
# > （鉄のモデルにおける「その他の血漿」と同一視される）の中央血液コンパートメントへ、
# > 以下の半減期で移行すると仮定されます：赤血球および血漿トランスフェリンについては1分、
# > 肝臓のコンパートメントについては2日、脾臓および赤色骨髄のコンパートメントについては
# > 30日、その他のコンパートメントについては7日です。 ...

  {Fe}/OtherPlasma        Blood1                    ---
  {Fe}/PlasmaTrans        Blood1                $(0.693 / (1/60/24))    # 半減期1分
  {Fe}/RBC                Blood1                $(0.693 / (1/60/24))    # 半減期1分
  {Fe}/Liver1(Hepato)     Blood1                $(0.693 /  2)           # 半減期2日
  {Fe}/Liver2(RE)         Blood1                $(0.693 /  2)           # 半減期2日
  {Fe}/Spleen             Blood1                $(0.693 / 30)           # 半減期30日
  {Fe}/MarrowSynthe       Blood1                $(0.693 / 30)           # 半減期30日
  {Fe}/MarrowTransit      Blood1                $(0.693 / 30)           # 半減期30日
  {Fe}/MarrowStorage      Blood1                $(0.693 / 30)           # 半減期30日
  {Fe}/Other1(Trans)      Blood1                $(0.693 /  7)           # 半減期7日
  {Fe}/Other2(Parenc)     Blood1                $(0.693 /  7)           # 半減期7日
  {Fe}/Other3(RE)         Blood1                $(0.693 /  7)           # 半減期7日

  {Co}/Blood1             Blood1                    ---
  {Co}/Blood2             Blood2                    ---
  {Co}/C-bone-S           C-bone-S                  ---
  {Co}/T-bone-S           T-bone-S                  ---
  {Co}/C-bone-V           C-bone-V                  ---
  {Co}/T-bone-V           T-bone-V                  ---
  {Co}/RedMarrow          RedMarrow                 ---
  {Co}/Liver1             Liver1                    ---
  {Co}/Liver2             Liver2                    ---
  {Co}/Kidneys1           Kidneys1                  ---
  {Co}/Kidneys2           Kidneys2                  ---
  {Co}/Spleen             Spleen                    ---
  {Co}/ST0                ST0                       ---
  {Co}/ST1                ST1                       ---
  {Co}/ST2                ST2                       ---

# ICRP Publ.134 p.169 Table 8.3
  Blood1                  Liver1                   70
  Blood1                  UB-con                   60
  Blood1                  RC-con                    4.0
 #Blood1                  ST0                      18       # fix
 #Blood1                  ST1                      10       # fix
  Blood1                  ST2                       4.0
  Blood1                  C-bone-S                  6.0
  Blood1                  T-bone-S                  6.0
  Blood1                  Kidneys1                  9.0
  Blood1                  Kidneys2                  1.0
  Blood1                  Blood2                   12
  Blood2                  Blood1                    0.693
  Liver1                  SI-conRe                  0.0924
  Liver1                  Blood1                    0.347
  Liver1                  Liver2                    0.0231
  Liver2                  Blood1                    0.0019
  ST0                     Blood1                    0.099
  ST1                     Blood1                    0.0139
  ST2                     Blood1                    0.00095
  C-bone-S                Blood1                    0.0842
  C-bone-S                C-bone-V                  0.0149
  T-bone-S                Blood1                    0.0842
  T-bone-S                T-bone-V                  0.0149
  C-bone-V                Blood1                    0.0000821
  T-bone-V                Blood1                    0.000493
  Kidneys1                UB-con                    0.462
  Kidneys2                Blood1                    0.0019

# > (298) ... The subsequent biokinetics of
# > cobalt entering or produced in the central blood compartment is described by the
# > systemic model for internally deposited cobalt (see the section on cobalt in this publication
# > (Section 8.2.3.)), with the following modifications for application to cobalt as
# > a progeny of iron. The spleen and red marrow are each added to the model as individual
# > compartments that exchange cobalt with the central blood compartment. These
# > compartments are assumed to receive 0.5% and 1% of outflow from the central blood
# > compartment, respectively. Depositions in the compartments of other soft tissues with
# > relatively fast and intermediate turnover rates are reduced from 9% and 5%, respectively,
# > in the original model to 8% and 4.5%, respectively. Cobalt is removed from the
# > spleen and red marrow to the central blood compartment with a half-time of 30 d.
# > (298) ... 中央血液コンパートメントに流入または
# > そこで生成されたコバルトのその後の体内動態は、内部沈着コバルトに関する全身モデル
# > （本出版物のコバルトの項（セクション8.2.3）を参照）によって記述されますが、
# > 鉄の娘核種としてのコバルトに適用するために以下の修正が加えられます。
# > 脾臓および赤色骨髄は、それぞれ中央血液コンパートメントとコバルトを交換する
# > 個別のコンパートメントとしてモデルに追加されます。これらのコンパートメントは、
# > 中央血液コンパートメントからの流出量のそれぞれ0.5%および1%を受け取ると仮定されます。
# > 比較的速いおよび中程度の代謝回転速度を持つその他の軟部組織コンパートメントへの沈着量は、
# > 元のモデルにおけるそれぞれ9%および5%から、それぞれ8%および4.5%に低減される。脾臓および赤色骨髄からの
# > コバルトは、半減期30日で中央血液コンパートメントへ除去される。

  Blood1                  Spleen                    1           # Blood1からの総流出(200 d⁻¹)の0.5%
  Blood1                  RedMarrow                 2           # Blood1からの総流出(200 d⁻¹)の1.0%
  Blood1                  ST0                      16           # Blood1からの総流出(200 d⁻¹)の9%→8%
  Blood1                  ST1                       9           # Blood1からの総流出(200 d⁻¹)の5%→4.5%

  Spleen                  Blood1                $(0.693 / 30)   # 半減期30日
  RedMarrow               Blood1                $(0.693 / 30)   # 半減期30日
