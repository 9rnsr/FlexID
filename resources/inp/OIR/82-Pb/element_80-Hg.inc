# ICRP Publ.137 p.229 Para.479 / 鉛子孫元素としての水銀
# > (479) The systemic model for mercury as a member of a lead chain consists of the
# > following compartments: Plasma 1 (diffusible mercury), Plasma 2 (protein-bound
# > mercury), red blood cells, kidneys, liver, spleen, red marrow (active marrow),
# > testes, ovaries, cortical bone surfaces, trabecular bone surfaces, and compartments
# > Other 1 and Other 2 representing moderate and slow phases of loss from remaining
# > soft tissues, respectively. ...
# > (479) 鉛連鎖の一員としての水銀の全身モデルは、以下のコンパートメントから構成されます。
# > 血漿1（拡散性水銀）、血漿 2（タンパク質結合水銀）、赤血球、腎臓、肝臓、脾臓、
# > 赤色骨髄（活性骨髄）、精巣、卵巣、皮質骨表面、海綿骨表面、および残存する軟部組織からの損失が
# > それぞれ中程度および低速な段階を表すコンパートメント「その他1」と「その他2」です。 ...
# > ,,, Mercury absorbed to blood or re-entering blood from tissues is assigned to Plasma 1.
# > ... 血液に吸収された水銀、または組織から血液に再流入する水銀は、血漿1に割り当てられます。
#
# 作成者注：Para.479の移行係数についての記述に「皮膚」の言及があるので、これも構成コンパートメントに含まれる。

[{Hg}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma1               Blood
  acc   Plasma2               Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
 #acc   Brain1                Brain     # Publ.151 only
 #acc   Brain2                Brain     # Publ.151 only
  acc   Other1                Other
  acc   Other2                Other
  exc   OtherExcreta          ---

# 作成者注：Publ.151で示された親としての水銀のモデルに対して、
# Brain1, Brain2がなく、代わりに以下のコンパートメントが追加されている。

  acc   RedMarrow             R-marrow
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen

[{Hg}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.229 Para.479 / 鉛子孫元素としての水銀
# > ... The total transfer coefficient from Plasma 1 to all destinations is 16.636 h⁻¹, corresponding
# > to a half-time of 1 h. Outflow from Plasma 1 is divided as follows: 4% to red
# > blood cells, 12% to Plasma 2, 35% to kidneys, 20% to liver, 10% to small intestine
# > contents, 3% to red marrow, 0.6% to spleen, 0.2% to skin, 0.03% to testes, 0.01% to
# > ovaries, 3.5% to Other 2, 5% to excreta (excretion other than urine and faeces), 1%
# > to cortical bone surfaces, 1% to trabecular bone surfaces, and the remaining 4.66%
# > to Other 1. Mercury transfers from red blood cells to Plasma 1 with a half-time of
# > 3 d, from Plasma 2 to Plasma 1 with a half-time of 1 d, from kidneys to urinary
# > bladder contents with a half-time of 35 d, from bone surface compartments to
# > Plasma 1 with a half-time of 20 d, from Other 1 to Plasma 1 with a half-time of
# > 20 d, and from Other 2 to Plasma 1 with a half-time of 100 d. Mercury is removed
# > from liver with a half-time of 10 d, with outflow from liver equally divided between
# > Plasma 1 and small intestine contents. Mercury transfers from red marrow, spleen,
# > skin, testes, and ovaries to Plasma 1 with a half-time of 20 d. Mercury is absorbed
# > from small intestine contents to Plasma 1 based on the reference absorption fraction
# > for ingested inorganic mercury, and the unabsorbed portion transfers to the right
# > colon contents and is eventually excreted in faeces.
# > ... 血漿1からすべての目的地への総移動係数は16.636 h⁻¹ で、半減期は1時間です。血漿1からの流出は、
# > 赤血球に4%、血漿2に12%、腎臓に35%、肝臓に20%、小腸内容物に10%、赤色骨髄に3%、脾臓に0.6%、
# > 皮膚に0.2%、精巣に0.03%、卵巣に0.01%、その他2に3.5%、排泄物（尿と便以外の排泄物）に5%、
# > 皮質骨表面に1%、海綿骨表面に1%、残りの4.66% がその他1に分配されます。水銀は、
# > 赤血球から血漿1へは半減期3日で、血漿2から血漿1へは半減期1日で、腎臓から膀胱内容物へは
# > 半減期35日、骨表面コンパートメントから血漿1へは半減期20日で、その他1から血漿1へは
# > 半減期20日で、その他2から血漿1へは半減期100日で移行します。
# > 水銀は、半減期10日で肝臓から除去され、肝臓からの流出は血漿1と小腸内容物に均等に分かれます。
# > 水銀は、赤色骨髄、脾臓、皮膚、精巣、卵巣から血漿1へ半減期20日で移行します。
# > 水銀は、摂取された無機水銀の基準吸収率に基づいて小腸内容物から血漿1に吸収され、
# > 吸収されなかった部分は右結腸内容物に移行し、最終的に糞便中に排泄されます。
#
# 作成者注：「Plasma1から全ての目的地への総移動係数の移行係数」は、半減期1時間=ln(2)/1/24=16.636 d⁻¹の誤記と思われる。

  Plasma1                 RBC                   $(16.636 * 4%)      # <- 0.48
  Plasma1                 Plasma2               $(16.636 * 12%)     # <- 2.4
  Plasma1                 Kidneys               $(16.636 * 35%)     # <- 7.2
  Plasma1                 Liver                 $(16.636 * 20%)     # <- 4.8
  Plasma1                 SI-conRe              $(16.636 * 10%)     # <- 1.92
  Plasma1                 RedMarrow             $(16.635 * 3%)
  Plasma1                 Spleen                $(16.635 * 0.6%)
  Plasma1                 Skin                  $(16.635 * 0.2%)
  Plasma1                 Testes                $(16.635 * 0.03%)
  Plasma1                 Ovaries               $(16.635 * 0.01%)
  Plasma1                 Other2                $(16.636 * 3.5%)    # <- 0.72
  Plasma1                 OtherExcreta          $(16.636 * 5%)      # <- 1.2
  Plasma1                 C-bone-S              $(16.636 * 1%)      # <- 0.024
  Plasma1                 T-bone-S              $(16.636 * 1%)      # <- 0.024
  Plasma1                 Other1                $(16.636 * 4.66%)   # <- 5.184

  RBC                     Plasma1               $(0.693 / 3)        # <- 0.33
  Plasma2                 Plasma1               $(0.693 / 1)        # <- 0.6
  Kidneys                 UB-con                $(0.693 / 35)       # == 0.0198
  T-bone-S                Plasma1               $(0.693 / 20)       # == 0.0347
  C-bone-S                Plasma1               $(0.693 / 20)       # == 0.0347
  Other1                  Plasma1               $(0.693 / 20)       # == 0.0347
  Other2                  Plasma1               $(0.693 / 100)      # == 0.00693
  Liver                   Plasma1               $(0.693 / 10 * 50%)                 # Publ.137 only
  Liver                   SI-conRe              $(0.693 / 10 * 50%) # <- 0.1733
  RedMarrow               Plasma1               $(0.693 / 20)
  Spleen                  Plasma1               $(0.693 / 20)
  Skin                    Plasma1               $(0.693 / 20)
  Testes                  Plasma1               $(0.693 / 20)
  Ovaries                 Plasma1               $(0.693 / 20)

# ICRP Publ.137 p.229 Para.479 / 鉛子孫元素としての水銀
# > ... Plasma 1, kidneys, and liver are identified with plasma,
# > Kidneys 1, and Liver 1, respectively, in the model for lead (Fig. 9.1).
# > ... 血漿1、腎臓、肝臓は、鉛のモデル（図9.1）では、それぞれ血漿、腎臓1、肝臓1として識別されます。
# > ... Mercury produced by radioactive
# > decay in a blood compartment that is not identifiable with a blood compartment of
# > the mercury model is assumed to transfer to Plasma 1 at the rate 1000 d⁻¹. Mercury
# > produced in a soft tissue compartment not identifiable with a compartment in the
# > model for mercury is assumed to transfer to Plasma 1 with a half-time of 20 d.
# > Mercury produced in a compartment of cortical or trabecular bone volume is
# > assumed to transfer to Plasma 1 at the reference turnover rate for that bone type.
# > ... 水銀モデルの血液コンパートメントと識別できない血液コンパートメントにおいて
# > 放射性崩壊によって生成された水銀は、1000 d⁻¹の速度で血漿1に移行すると仮定されます。
# > 水銀モデルのコンパートメントと識別できない軟組織コンパートメントで生成された水銀は、
# > 半減期20日で血漿1に移行すると仮定されます。皮質骨または海綿骨の体積コンパートメントで
# > 生成された水銀は、その骨の種類における基準代謝率で血漿1に移行すると仮定されます。

  {Po}/Plasma1            Plasma1                1000
  {Po}/Plasma2            Plasma1                1000
  {Po}/Plasma3            Plasma1                1000
  {Bi,Pb,Tl}/Plasma       Plasma1                   ---
  {Hg}/Plasma1            Plasma1                   ---
  {Hg}/Plasma2            Plasma2                   ---
  {Po,Bi,Pb,Tl,Hg}/RBC    RBC                       ---
  {Au}/Blood1             Plasma1                   ---
  {Au}/Blood2             Plasma1                1000

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {Pb}/Exch-C-bone-V      Plasma1               $(0.03 / 365)
  {Pb}/Exch-T-bone-V      Plasma1               $(0.18 / 365)
  {Pb}/Noch-C-bone-V      Plasma1               $(0.03 / 365)
  {Pb}/Noch-T-bone-V      Plasma1               $(0.18 / 365)

  {Po,Bi,Pb}/Liver1       Liver                     ---
  {Po,Bi,Pb}/Liver2       Plasma1               $(0.693 / 20)
  {Tl,Hg,Au}/Liver        Liver                     ---
  {Po,Bi,Pb}/Kidneys1     Kidneys                   ---
  {Po,Bi,Pb}/Kidneys2     Plasma1               $(0.693 / 20)
  {Tl}/Kidneys            Plasma1               $(0.693 / 20)
  {Hg,Au}/Kidneys         Kidneys                   ---
  {*}/RedMarrow           RedMarrow                 ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Hg}/Other1             Other1                    ---
  {Hg}/Other2             Other2                    ---
  {^Hg}/Other1            Plasma1               $(0.693 / 20)
  {^Hg}/Other2            Plasma1               $(0.693 / 20)
  {*}/ST0                 Plasma1               $(0.693 / 20)
  {*}/ST1                 Plasma1               $(0.693 / 20)
  {*}/ST2                 Plasma1               $(0.693 / 20)
  {*}/Other               Plasma1               $(0.693 / 20)
