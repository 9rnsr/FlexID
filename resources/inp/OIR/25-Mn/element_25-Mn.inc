[{Mn}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Brain                 Brain
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Mn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.123 Table 14.3
  Plasma                  Liver1                  300
  Plasma                  Kidneys                  50
  Plasma                  Pancreas                 50
  Plasma                  UB-con                    2
  Plasma                  RC-con                   10
  Plasma                  ST0                     391.8
  Plasma                  ST1                     150
  Plasma                  ST2                      40
  Plasma                  C-bone-S                  2.5
  Plasma                  T-bone-S                  2.5
  Plasma                  Brain                     1.0
  Plasma                  RBC                       0.2
  Liver1                  SI-conRe                  0.139
  Liver1                  Liver2                    0.555
  Liver2                  Plasma                    0.347
  Kidneys                 Plasma                    0.347
  Pancreas                Plasma                    0.347
  Pancreas                SI-conRe                  0.347
  ST0                     Plasma                   33.3
  ST1                     Plasma                    0.347
  ST2                     Plasma                    0.0173
  C-bone-S                Plasma                    0.01716
  C-bone-S                C-bone-V                  0.0001733
  T-bone-S                Plasma                    0.01716
  T-bone-S                T-bone-V                  0.0001733
  C-bone-V                Plasma                    0.0000821
  T-bone-V                Plasma                    0.000493
  Brain                   Plasma                    0.00462
  RBC                     Plasma                    0.00833

$if progeny

# > (226) Progeny of manganese addressed in this publication are radioisotopes of
# > manganese and chromium. The model for manganese as a parent is applied to manganese
# > produced by decay of another manganese isotope. The model for chromium
# > as progeny of manganese is an expansion of the characteristic model for chromium
# > with added compartments and associated transfer coefficients needed to solve the
# > linked biokinetic models for manganese and chromium (see Annex B). If produced in
# > a compartment not explicitly named in the model for chromium, chromium is
# > assumed to transfer at the following rates: 1000 d⁻¹ if produced in a blood compartment;
# > at the rate of bone turnover if produced in a bone volume compartment; and
# > at 0.25 d⁻¹ if produced in any other compartment.
# > (226) 本書で扱うマンガンの娘核種は、マンガンおよびクロムの放射性同位体です。親核種が
# > マンガンである場合のモデルは、別のマンガン同位体の崩壊によって生成されたマンガンに対しても
# > 適用されます。マンガンの娘核種としてのクロムに関するモデルは、クロムの標準的なモデルを
# > 拡張したものであり、マンガンとクロムの連動した生物動態モデルを解くために必要な
# > コンパートメントおよびそれに関連する移行係数が追加されています（付録Bを参照）。クロムの
# > モデルにおいて明示的に指定されていないコンパートメント内でクロムが生成される場合、
# > その移行速度は次のように仮定されます。すなわち、血液コンパートメント内で生成される場合は
# > 1000 d⁻¹、骨体積コンパートメント内で生成される場合は骨代謝回転速度、その他の
# > コンパートメント内で生成される場合は0.25 d⁻¹です。

  {Mn}/Plasma             Plasma                    ---
  {Mn}/RBC                RBC                       ---

  {Mn}/C-bone-S           C-bone-S                  ---
  {Mn}/T-bone-S           T-bone-S                  ---
  {Mn}/C-bone-V           C-bone-V                  ---
  {Mn}/T-bone-V           T-bone-V                  ---

  {Mn}/Brain              Brain                     ---
  {Mn}/Liver1             Liver1                    ---
  {Mn}/Liver2             Liver2                    ---
  {Mn}/Kidneys            Kidneys                   ---
  {Mn}/Pancreas           Pancreas                  ---

  {Mn}/ST0                ST0                       ---
  {Mn}/ST1                ST1                       ---
  {Mn}/ST2                ST2                       ---

$endif
