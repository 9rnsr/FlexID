# ICRP Publ.137 p.231 Para.484 / 鉛子孫元素としてのポロニウム
# > (484) The systemic model for polonium as a member of a lead chain is the same as
# > the systemic model applied in this publication to polonium as a parent radionuclide
# > (Section 11), except that no distinction is made in the progeny model between
# > polonium entering blood from the respiratory and alimentary tracts. ...
# > (484) 鉛鎖の一員としてのポロニウムの全身モデルは、本書で親核種としてのポロニウムに
# > 適用されている全身モデル(第11節)と同一です。ただし、子孫核種モデルでは、呼吸器系と
# > 消化器系から血液に入るポロニウムを区別しない点が異なります。 ...

[{Po}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma1               Blood
  acc   Plasma2               Blood
  acc   Plasma3               Blood
  acc   RBC                   Blood
  mix   BoneSurface           ---           # Publ.137 Para.605
  acc   C-bone-S              C-bone-S      # Publ.137 Para.605
  acc   T-bone-S              T-bone-S      # Publ.137 Para.605
  acc   RedMarrow             R-marrow
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissues
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   Skin                  Skin
  acc   Spleen                Spleen
  acc   Other                 Other
  exc   Excreta               ---           # hair,sweat,skin

[{Po}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.137 p.231 Para.484 / 鉛子孫元素としてのポロニウム
# > ... polonium as a progeny transferred to blood from either the respiratory
# > or alimentary tract is assigned to Plasma 2.
# > ... 呼吸器系または消化器系から血液に移行する子孫核種としてのポロニウムは、
# > 血漿2に割り当てられます。

# ICRP Publ.137 p.285 Table 11.3
  Plasma2                 Plasma1                 800
  Plasma2                 Kidneys1                200
  Plasma1                 Plasma3                   4
  Plasma1                 RBC                       6
  Plasma1                 Liver1                   17.5
  Plasma1                 Liver2                   17.5
  Plasma1                 Kidneys1                  5
  Plasma1                 Kidneys2                  5
  Plasma1                 Skin                      5
  Plasma1                 RedMarrow                 4
  Plasma1                 BoneSurface               1.5
  Plasma1                 Spleen                    2
  Plasma1                 Testes                    0.1
  Plasma1                 Ovaries                   0.05
  Plasma1                 Other                    32.35
  Plasma3                 Plasma1                   0.099
  RBC                     Plasma1                   0.099
  Liver1                  SI-conRe                  0.139
  Liver2                  Plasma1                   0.099
  Kidneys1                UB-con                    0.173
  Kidneys2                Plasma1                   0.099
  Skin                    Plasma1                   0.00693
  Skin                    Excreta                   0.00693
  RedMarrow               Plasma1                   0.099
  BoneSurface             C-bone-S                 50%          # Publ.137 Para.605
  BoneSurface             T-bone-S                 50%          # Publ.137 Para.605
  C-bone-S                Plasma1                   0.0231      # Publ.137 Para.605
  T-bone-S                Plasma1                   0.0231      # Publ.137 Para.605
  Spleen                  Plasma1                   0.099
  Testes                  Plasma1                   0.0139
  Ovaries                 Plasma1                   0.0139
  Other                   Plasma1                   0.099

# ICRP Publ.137 p.231 Para.484 / 鉛子孫元素としてのポロニウム
# > ... Polonium produced in a blood compartment
# > that is not identifiable with a blood compartment of the polonium model is
# > assumed to transfer to Plasma 2 in the polonium model (Fig. 11.1) at the rate
# > 1000 d⁻¹. Polonium produced in a soft tissue compartment not identifiable with a
# > compartment of the polonium model is assumed to move to Plasma 2 with a halftime
# > of 7 d. Polonium produced in a compartment of cortical or trabecular bone
# > volume is assumed to transfer to Plasma 2 at the reference rate of turnover of that
# > bone type.
# > ... ポロニウムモデルの血液コンパートメントと識別できない血液コンパートメントで
# > 生成されたポロニウムは、ポロニウムモデル(図11.1)において、
# > 1000 d⁻¹の速度で血漿2に移行すると仮定されます。ポロニウムモデルのコンパートメントに
# > 識別できない軟部組織コンパートメントで生成されたポロニウムは、7日の半減期で血漿2に
# > 移動するものと仮定されます。皮質骨または海綿骨の体積コンパートメントで生成された
# > ポロニウムは、その骨の種類に応じた基準代謝速度で血漿2に移行するものと仮定されます。

  {Po}/Plasma1            Plasma1                   ---
  {Po}/Plasma2            Plasma2                   ---
  {Po}/Plasma3            Plasma3                   ---
  {Bi,Pb,Tl}/Plasma       Plasma2                1000
  {Hg}/Plasma1            Plasma2                1000
  {Hg}/Plasma2            Plasma2                1000
  {Po,Bi,Pb,Tl,Hg}/RBC    RBC                       ---
  {Au}/Blood1             Plasma2                1000
  {Au}/Blood2             Plasma2                1000

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {Pb}/Exch-C-bone-V      Plasma2               $(0.03 / 365)
  {Pb}/Exch-T-bone-V      Plasma2               $(0.18 / 365)
  {Pb}/Noch-C-bone-V      Plasma2               $(0.03 / 365)
  {Pb}/Noch-T-bone-V      Plasma2               $(0.18 / 365)

  {Po,Bi,Pb}/Liver1       Liver1                    ---
  {Po,Bi,Pb}/Liver2       Liver2                    ---
  {Tl,Hg,Au}/Liver        Liver1                    ---
  {Po,Bi,Pb}/Kidneys1     Kidneys1                  ---
  {Po,Bi,Pb}/Kidneys2     Kidneys2                  ---
  {Tl}/Kidneys            Kidneys2                  ---
  {Hg,Au}/Kidneys         Kidneys1                  ---
  {*}/RedMarrow           RedMarrow                 ---
  {*}/Testes              Testes                    ---
  {*}/Ovaries             Ovaries                   ---
  {*}/Skin                Skin                      ---
  {*}/Spleen              Spleen                    ---

  {Po}/Other              Other                     ---
  {^Po}/Other             Plasma2               $(0.693 / 7)
  {*}/ST0                 Plasma2               $(0.693 / 7)
  {*}/ST1                 Plasma2               $(0.693 / 7)
  {*}/ST2                 Plasma2               $(0.693 / 7)
  {*}/Other1              Plasma2               $(0.693 / 7)
  {*}/Other2              Plasma2               $(0.693 / 7)
