# ICRP Publ.137 p.227 Para.477 / 鉛子孫元素としての金
# > (477) The following systemic model is applied to gold produced in vivo following
# > intake of lead. ...
# > (477) 鉛摂取後に体内で生成される金には、以下の全身モデルが適用されます。 ...

[{Au}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other1                Other
  acc   Other2                Other

# 作成者注：Publ.151で示された親としての金のモデルに対して追加

  acc   RedMarrow             R-marrow
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen

[{Au}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.227 Para.477 / 鉛子孫元素としての金
# >  ... Gold leaves the central blood compartment (Blood 1) at the rate 1d 1
# > and is distributed as follows: 10% to Blood 2 (a blood compartment with relatively
# > slow turnover), 30% to urinary bladder contents, 10% to right colon contents, 10%
# > to kidneys, 10% to liver, 5% to red marrow, 1% to spleen, 1% to trabecular bone
# > surfaces, 1% to cortical bone surfaces, 0.5% to skin, 0.06% to testes, 0.02% to
# > ovaries, 10% to Other 2 (a soft tissue compartment with slow turnover), and the
# > remaining 11.42% to Other 1 (a soft tissue compartment with a moderate turnover
# > rate).
# > ... Gold transfers from Blood 2 to Blood 1
# > with a half-time of 5d; from liver, Other 1, spleen, testes, ovaries, red marrow, skin,
# > trabecular bone surfaces, and cortical bone surfaces to Blood 1 with a half-time of
# > 10d; from kidneys to urinary bladder contents with a half-time of 10d; and from
# > Other 2 to blood with a half-time of 50d.
# > ... 金は中心血液コンパートメント（血液1）から1d 1の速度で排出され、以下のように分布します：
# > 10%が血液2（比較的代謝の遅い血液コンパートメント）、30%が膀胱内容物、10%が右結腸内容物、
# > 10%が腎臓、10%が肝臓、5%が赤色骨髄、1%が脾臓、1%が海綿骨表面、1%が皮質骨表面、0.5%が皮膚、
# > 0.06%が精巣、0.02%が卵巣、10%がその他2（代謝の遅い軟部組織コンパートメント）、
# > 残りの11.42%がその他1（中程度の代謝速度の軟部組織コンパートメント）に分配されます。
# > .... 金は血液2から血液1へ、半減期5日で移行します。肝臓、その他1、脾臓、精巣、卵巣、
# > 赤色骨髄、皮膚、海綿骨表面、皮質骨表面から血液1へは半減期10日で移行します。
# > 腎臓から膀胱内容物へは半減期10日で移行します。その他2から血液へは半減期50日で移行します。

  Blood1                  Blood2                $(1 * 10%)
  Blood1                  Kidneys               $(1 * 10%)
  Blood1                  Liver                 $(1 * 10%)
 #Blood1                  Other1                $(1 * 18%)      # == 0.18   # fix
  Blood1                  Other1                $(1 * 11.42%)   # == 0.1142
  Blood1                  Other2                $(1 * 10%)
  Blood1                  UB-con                $(1 * 30%)
  Blood1                  RC-con                $(1 * 10%)
  Blood1                  T-bone-S              $(1 *  1%)
  Blood1                  C-bone-S              $(1 *  1%)
  Blood2                  Blood1                    0.139       # ln(2) / 5
  Kidneys                 UB-con                    0.0693      # ln(2) / 10
  Liver                   Blood1                    0.0693      # ln(2) / 10
  Other1                  Blood1                    0.0693      # ln(2) / 10
  Other2                  Blood1                    0.0139      # ln(2) / 50
  T-bone-S                Blood1                    0.0693      # ln(2) / 10
  C-bone-S                Blood1                    0.0693      # ln(2) / 10

# 作成者注：Publ.137で示されている移行係数の設定値は、Publ.151で示された親としての
# 金のモデルにおけるBlood1とOther1の間の流量を、追加された5つのコンパートメントに
# 分配する形になっています。

  Blood1                  RedMarrow             $(1 *  5%)
  Blood1                  Testes                $(1 *  0.06%)
  Blood1                  Ovaries               $(1 *  0.02%)
  Blood1                  Skin                  $(1 *  0.5%)
  Blood1                  Spleen                $(1 *  1%)
  Spleen                  Blood1                    0.0693      # ln(2) / 10
  Testes                  Blood1                    0.0693      # ln(2) / 10
  Ovaries                 Blood1                    0.0693      # ln(2) / 10
  RedMarrow               Blood1                    0.0693      # ln(2) / 10
  Skin                    Blood1                    0.0693      # ln(2) / 10

# ICRP Publ.137 p.227 Para.477 / 鉛子孫元素としての金
# > ... Blood 1, kidneys, and liver are identified with plasma, Kidneys 1, and Liver 1,
# > respectively, in the model for lead (Fig. 9.1).
# > ... Gold produced by radioactive decay in a
# > blood compartment that is not identifiable with a blood compartment of the gold
# > model is assumed to transfer to Blood 1 at the rate 1000 d⁻¹. Gold produced in a soft
# > tissue compartment not identifiable with a compartment in the model for gold is
# > assumed to transfer to Blood 1 with a half-time of 10d. Gold produced in a com
# > partment of trabecular or cortical bone volume is assumed to transfer to Blood 1 at
# > the reference turnover rate for that bone type.
# > ... 鉛のモデルでは、血液1、腎臓、肝臓はそれぞれ血漿、腎臓1、肝臓1に相当します（図9.1）。
# > ... 金モデルの血液コンパートメントと識別できない血液コンパートメント内で
# > 放射性崩壊によって生成された金は、1000 d⁻¹の速度で血液1に移行すると仮定されます。
# > 金モデルのコンパートメントと識別できない軟部組織コンパートメントで生成された金は、
# > 半減期10日で血液1に移行すると仮定されます。海綿骨または皮質骨の体積コンパートメントで
# > 生成された金は、その骨の種類に応じた基準代謝速度で血液1に移行すると仮定されます。

  {Po}/Plasma1            Blood1                 1000
  {Po}/Plasma2            Blood1                 1000
  {Po}/Plasma3            Blood1                 1000
  {Bi,Pb,Tl}/Plasma       Blood1                    ---
  {Hg}/Plasma1            Blood1                    ---
  {Hg}/Plasma2            Blood1                 1000
  {Po,Bi,Pb,Tl,Hg}/RBC    Blood1                 1000
  {Au}/Blood1             Blood1                    ---
  {Au}/Blood2             Blood2                    ---

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {Pb}/Exch-C-bone-V      Blood1                $(0.03 / 365)
  {Pb}/Exch-T-bone-V      Blood1                $(0.18 / 365)
  {Pb}/Noch-C-bone-V      Blood1                $(0.03 / 365)
  {Pb}/Noch-T-bone-V      Blood1                $(0.18 / 365)

  {Po,Bi,Pb}/Liver1       Liver                     ---
  {Po,Bi,Pb}/Liver2       Blood1                $(0.693 / 10)
  {Tl,Hg,Au}/Liver        Liver                     ---
  {Po,Bi,Pb}/Kidneys1     Kidneys                   ---
  {Po,Bi,Pb}/Kidneys2     Blood1                $(0.693 / 10)
  {Tl}/Kidneys            Blood1                $(0.693 / 10)
  {Hg,Au}/Kidneys         Kidneys                   ---
  {*}/RedMarrow           RedMarrow                 ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Au}/Other1             Other1                    ---
  {Au}/Other2             Other2                    ---
  {^Au}/Other1            Blood1                $(0.693 / 10)
  {^Au}/Other2            Blood1                $(0.693 / 10)
  {*}/ST0                 Blood1                $(0.693 / 10)
  {*}/ST1                 Blood1                $(0.693 / 10)
  {*}/ST2                 Blood1                $(0.693 / 10)
  {*}/Other               Blood1                $(0.693 / 10)
