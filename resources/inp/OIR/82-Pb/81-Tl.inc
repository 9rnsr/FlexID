# ICRP Publ.137 p.229 Para.481 / 鉛子孫元素としてのタリウム
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

  acc   RedMarrow             R-marrow
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen

[{Tl}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.229 Para.481 / 鉛子孫元素としてのタリウム
# > ... Thallium leaves the central blood compartment (plasma) at the
# > rate 200 d⁻¹ (corresponding to a half-time of 5 min) and is distributed as follows:
# > 2.5% to red blood cells, 0.75% to urinary bladder contents, 1.75% to right colon
# > contents, 5% to kidneys, 5% to liver, 2.5% to skin, 1.5% to red marrow, 0.2% to
# > spleen, 0.3% to testes, 0.1% to ovaries, 7.5% to trabecular bone surfaces, 7.5% to
# > cortical bone surfaces, and 65.4% to remaining soft tissues (other).
# > ... タリウムは中心血液区画（血漿）から200 d⁻¹（半減期5分に相当）の速度で排出され、
# > 以下のように分布します：赤血球に2.5%、膀胱内容物に0.75%、右結腸内容物に1.75%、腎臓に5%、
# > 肝臓に5%、皮膚に2.5%、赤色骨髄に1.5%、脾臓に0.2%、精巣に0.3%、卵巣に0.1%、
# > 海綿骨表面に7.5%、皮質骨表面に7.5%、残りの軟部組織（その他）に65.4%。
# > ... Thallium returns from red blood cells to plasma at the rate 3.7 d⁻¹, and from
# > tissue compartments to plasma at the rate 2.5 d⁻¹.
# > ... タリウムは、赤血球から血漿へは3.7 d⁻¹の速度で戻り、組織コンパートメントから
# > 血漿へは2.5 d⁻¹の速度で戻ります。

# 作成者注：Publ.137で示されている移行係数の設定値は、Publ.151で示された親としての
# タリウムのモデルにおけるPlasmaと軟組織Otherの間の流量を、追加された
# 5つのコンパートメントに分配する形になっている。

  Plasma                  Liver                 $(200 *  5%)
  Plasma                  Kidneys               $(200 *  5%)
  Plasma                  RBC                   $(200 *  2.5%)
  Plasma                  T-bone-S              $(200 *  7.5%)
  Plasma                  C-bone-S              $(200 *  7.5%)
 #Plasma                  Other                 $(200 * 70%)    # == 140    # fix
  Plasma                  Other                 $(200 * 65.4%)  # == 130.8
  Plasma                  RedMarrow             $(200 *  1.5%)
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
  RedMarrow               Plasma                    2.5
  Skin                    Plasma                    2.5
  Spleen                  Plasma                    2.5
  Testes                  Plasma                    2.5
  Ovaries                 Plasma                    2.5

# ICRP Publ.137 p.229 Para.481 / 鉛子孫元素としてのタリウム
# > ... Kidneys and liver are identified with Kidneys 2 and Liver 1, respectively,
# > in the model for lead (Fig. 9.1).
# > ... 腎臓と肝臓は、鉛のモデル（図9.1）において、それぞれ腎臓2と肝臓1に相当します。
# > ... Thallium produced by radioactive
# > decay in a blood compartment that is not identifiable with a blood compartment of
# > the thallium model is assumed to transfer to plasma at the rate 1000 d⁻¹. Thallium
# > produced in a soft tissue compartment not identifiable with a compartment of the
# > thallium model is assumed to transfer to plasma at the rate 2.5 d⁻¹. Thallium produced
# > in a compartment of cortical or trabecular bone volume is assumed to transfer
# > to plasma at the reference turnover rate of that bone type.
# > ... タリウムモデルの血液コンパートメントと識別できない血液コンパートメント内で
# > 放射性崩壊によって生成されたタリウムは、1000 d⁻¹の速度で血漿に移行すると仮定されます。
# > タリウムモデルのコンパートメントと識別できない軟部組織コンパートメントで生成された
# > タリウムは、2.5 d⁻¹の速度で血漿に移行すると仮定されます。皮質骨または海綿骨の
# > 体積コンパートメントで生成されたタリウムは、その骨の種類に応じた基準代謝速度で
# > 血漿に移行すると仮定されます。

  {Po}/Plasma1            Plasma                 1000
  {Po}/Plasma2            Plasma                 1000
  {Po}/Plasma3            Plasma                 1000
  {Bi,Pb,Tl}/Plasma       Plasma                    ---
  {Hg}/Plasma1            Plasma                    ---
  {Hg}/Plasma2            Plasma                 1000
  {Po,Bi,Pb,Tl,Hg}/RBC    RBC                       ---
  {Au}/Blood1             Plasma                    ---
  {Au}/Blood2             Plasma                 1000

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {Pb}/Exch-C-bone-V      Plasma                $(0.03 / 365)
  {Pb}/Exch-T-bone-V      Plasma                $(0.18 / 365)
  {Pb}/Noch-C-bone-V      Plasma                $(0.03 / 365)
  {Pb}/Noch-T-bone-V      Plasma                $(0.18 / 365)

  {Po,Bi,Pb}/Liver1       Liver                     ---
  {Po,Bi,Pb}/Liver2       Plasma                    2.5
  {Tl,Hg,Au}/Liver        Liver                     ---
  {Po,Bi,Pb}/Kidneys1     Plasma                    2.5
  {Po,Bi,Pb}/Kidneys2     Kidneys                   ---
  {Tl}/Kidneys            Kidneys                   ---
  {Hg,Au}/Kidneys         Plasma                    2.5
  {*}/RedMarrow           RedMarrow                 ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Tl}/Other              Other                     ---
  {^Tl}/Other             Plasma                    2.5
  {*}/ST0                 Plasma                    2.5
  {*}/ST1                 Plasma                    2.5
  {*}/ST2                 Plasma                    2.5
  {*}/Other1              Plasma                    2.5
  {*}/Other2              Plasma                    2.5
