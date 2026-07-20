#    ICRP Publ.137 p.370 Para.812 / トリウム系列子孫元素としてのタリウムは、ラジウム子孫元素としてのモデルを用いる
# → ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としてのタリウムには、鉛子孫元素としてのモデルを修正して用いる
#    ICRP Publ.137 p.229 Para.481 / 鉛子孫元素としてのタリウム
# > (481) The following systemic model is applied to thallium produced in vivo following
# > intake of lead. ...
# > (481) 鉛摂取後に体内で生成されるタリウムには、以下の全身モデルが適用されます。 ...

[{Tl}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other                 Other

# 作成者注：Publ.151で示された親としてのタリウムのモデルに対して追加

 #acc   RedMarrow             R-marrow      # Publ.137 Para.712
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen

# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としてのタリウム
# > (712) ... For each of these four elements, the red marrow compartment
# > is replaced by a compartment with the same kinetics (i.e. the same paths and rates of
# > inflow and outflow) named ‘trabecular marrow’ for consistency with the models for
# > other radium progeny; a compartment is added to represent cortical marrow, which is
# > assumed to exchange activity with plasma; ...
# > (712) ... これら4元素それぞれに
# > ついて、赤色骨髄コンパートメントは、他のラジウム子孫元素のモデルとの整合性を図るため、
# > 同じ動態（すなわち、同じ経路および流入・流出速度）を持つ「骨梁骨髄」と呼ばれる
# > コンパートメントに置き換えられる。皮質骨髄を表すコンパートメントが追加され、この
# > コンパートメントは血漿と放射能を交換すると仮定される。...

  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow

[{Tl}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.229 Para.481 / 鉛子孫元素としてのタリウム
# > (481) ... Thallium leaves the central blood compartment (plasma) at the
# > rate 200 d⁻¹ (corresponding to a half-time of 5 min) and is distributed as follows:
# > 2.5% to red blood cells, 0.75% to urinary bladder contents, 1.75% to right colon
# > contents, 5% to kidneys, 5% to liver, 2.5% to skin, 1.5% to red marrow, 0.2% to
# > spleen, 0.3% to testes, 0.1% to ovaries, 7.5% to trabecular bone surfaces, 7.5% to
# > cortical bone surfaces, and 65.4% to remaining soft tissues (other). Kidneys and liver
# > are identified with Kidneys 2 and Liver 1, respectively, in the model for lead (Fig.
# > 9.1). Thallium returns from red blood cells to plasma at the rate 3.7 d⁻¹, and from
# > tissue compartments to plasma at the rate 2.5 d⁻¹. ...
# > (481) ... タリウムは
# > 中心血液区画（血漿）から200日（半減期5分に相当）の速度で排出され、以下のように分布する：
# > 赤血球に2.5%、膀胱内容物に0.75%、右結腸内容物に1.75%、腎臓に5%、肝臓に5%、皮膚に2.5%、
# > 赤色骨髄に1.5%、脾臓に0.2%、精巣に0.3%、卵巣に0.1%、海綿骨表面に7.5%、皮質骨表面に7.5%、
# > 残りの軟部組織（その他）に65.4%。腎臓と肝臓は、鉛のモデル（図9.1）において、それぞれ
# > 腎臓2と肝臓1に相当します。タリウムは、赤血球から血漿へは3.7 d⁻¹の速度で戻り、
# > 組織コンパートメントから血漿へは2.5 d⁻¹の速度で戻ります。 ...

# 作成者注：Publ.137で示されている移行係数の設定値は、Publ.151で示された親としての
# タリウムのモデルにおおけるPlasmaと軟組織Otherの間の流量を、追加された
# 5つのコンパートメントに分配する形になっている。

  Plasma                  Liver                 $(200 *  5%)
  Plasma                  Kidneys               $(200 *  5%)
  Plasma                  RBC                   $(200 *  2.5%)
  Plasma                  T-bone-S              $(200 *  7.5%)
  Plasma                  C-bone-S              $(200 *  7.5%)
 #Plasma                  Other                 $(200 * 70%)    # == 140    # fix(1)
 #Plasma                  Other                 $(200 * 65.4%)  # == 130.8  # fix(2)
 #Plasma                  RedMarrow             $(200 *  1.5%)              # fix(2)
  Plasma                  Skin                  $(200 *  2.5%)
  Plasma                  Spleen                $(200 *  0.2%)
  Plasma                  Testes                $(200 *  0.3%)
  Plasma                  Ovaries               $(200 *  0.1%)
  Plasma                  UB-con                $(200 *  0.75%)
  Plasma                  RC-con                $(200 *  1.75%)
  RBC                     Plasma                    3.7
  Liver                   Plasma                    2.5
  Kidneys                 Plasma                    2.5
  T-bone-S                Plasma                    2.5
  C-bone-S                Plasma                    2.5
  Other                   Plasma                    2.5
 #RedMarrow               Plasma                    2.5                     # fix(2)
  Skin                    Plasma                    2.5
  Spleen                  Plasma                    2.5
  Testes                  Plasma                    2.5
  Ovaries                 Plasma                    2.5

# ICRP Publ.137 p.331 Para.712 / ラジウム子孫元素としてのタリウム
# > (712) ... the transfer coefficient from plasma to
# > cortical marrow is one-third of the transfer coefficient from plasma to trabecular
# > marrow; the removal half-time from cortical marrow is the same as that for trabecular
# > marrow; and the transfer coefficient from plasma to ST1 is reduced to maintain the
# > original total outflow rate from plasma to all destinations.
# > (712) ... 血漿から皮質骨髄への移行係数は、
# > 血漿から骨梁骨髄への移行係数の3分の1である。皮質骨髄からの除去半減期は、骨梁骨髄の場合と
# > 同じである。血漿からST1への移行係数は、血漿からすべての目的地への元の総流出速度を
# > 維持するために低減される。

  Plasma                  T-marrow              $(200 *  1.5%)      # RedMarrowをT-marrowに置換
  Plasma                  C-marrow              $(200 *  0.5%)      # Plasma→T-marrowの移行速度の1/3
  Plasma                  Other                 $(200 * 64.9%)      # Plasma→C-marrowの速度を差し引く
  T-marrow                Plasma                    2.5             # RedMarrowをT-marrowに置換
  C-marrow                Plasma                    2.5             # T-marrow→Plasmaの移行速度と同じ

# ICRP Publ.137 p.229 Para.481 / 鉛子孫元素としてのタリウム
# > (481) ... Thallium produced by radioactive
# > decay in a blood compartment that is not identifiable with a blood compartment of
# > the thallium model is assumed to transfer to plasma at the rate 1000 d⁻¹. Thallium
# > produced in a soft tissue compartment not identifiable with a compartment of the
# > thallium model is assumed to transfer to plasma at the rate 2.5 d⁻¹. Thallium produced
# > in a compartment of cortical or trabecular bone volume is assumed to transfer
# > to plasma at the reference turnover rate of that bone type.
# > (481) ... タリウムモデルの
# > 血液コンパートメントと識別できない血液コンパートメント内で放射性崩壊によって生成された
# > タリウムは、1000 d⁻¹の速度で血漿に移行すると仮定されます。タリウムモデルのコンパートメントと
# > 識別できない軟部組織コンパートメントで生成されたタリウムは、2.5 d⁻¹の速度で
# > 血漿に移行すると仮定されます。皮質骨または海綿骨の容積コンパートメントで生成されたタリウムは、
# > その骨の種類の基準代謝速度で血漿に移行すると仮定されます。

  {*}/Plasma              Plasma                    ---
  {Po}/Plasma1            Plasma                 1000
  {Po}/Plasma2            Plasma                 1000
  {Po}/Plasma3            Plasma                 1000
  {*}/RBC                 RBC                       ---
  {Pu}/Blood0             Plasma                 1000
  {Pu}/Blood1             Plasma                 1000
  {Pu}/Blood2             Plasma                 1000
  {*}/Blood               Plasma                 1000

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {*}/C-bone-V            Plasma                $(0.03 / 365)
  {*}/T-bone-V            Plasma                $(0.18 / 365)
  {*}/Noch-C-bone-V       Plasma                $(0.03 / 365)
  {*}/Noch-T-bone-V       Plasma                $(0.18 / 365)
  {*}/Exch-C-bone-V       Plasma                $(0.03 / 365)
  {*}/Exch-T-bone-V       Plasma                $(0.18 / 365)

  {*}/C-marrow            C-marrow                  ---
  {*}/T-marrow            T-marrow                  ---
  {Pu}/Liver0             Plasma                    2.5
  {Pu}/Liver1             Plasma                    2.5
  {Pu}/Liver2             Plasma                    2.5
  {^Pu}/Liver1            Liver                     ---
  {^Pu}/Liver2            Plasma                    2.5
  {*}/Kidneys1            Plasma                    2.5
  {*}/Kidneys2            Kidneys                   ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Pu}/ST0                Plasma                    2.5
  {Pu}/ST1                Plasma                    2.5
  {Pu}/ST2                Plasma                    2.5
  {^Pu}/ST0               Plasma                    2.5
  {^Pu}/ST1               Plasma                    2.5
  {^Pu}/ST2               Plasma                    2.5
  {Tl}/Other              Other                     ---
  {^Tl}/Other             Plasma                    2.5
