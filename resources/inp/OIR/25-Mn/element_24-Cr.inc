[{Cr}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma1               Blood
  acc   Plasma2               Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Other1                Other
  acc   Other2                Other


[{Cr}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.114 Table 13.3
  Plasma1                 Plasma2                 220
  Plasma1                 UB-con                    4.8
  Plasma2                 Plasma1                  10
  Plasma2                 RC-con                    0.1
  Plasma2                 Other1                    0.7
  Plasma2                 Other2                    0.027
  Plasma2                 Kidneys                   0.015
  Plasma2                 Liver                     0.15
  Plasma2                 T-bone-S                  0.01
  Plasma2                 C-bone-S                  0.01
  Other1                  Plasma1                   0.25
  Other2                  Plasma1                   0.00005
  Liver                   Plasma1                   0.01
  Kidneys                 Plasma1                   0.007
  T-bone-S                Plasma1                   0.000493
  C-bone-S                Plasma1                   0.0000821

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
# > (226) 本書で扱うマンガンの娘核種は、マンガンおよびクロムの放射性同位体である。親核種が
# > マンガンである場合のモデルは、別のマンガン同位体の崩壊によって生成されたマンガンに対しても
# > 適用される。マンガンの娘核種としてのクロムに関するモデルは、クロムの標準的なモデルを
# > 拡張したものであり、マンガンとクロムの連動した生物動態モデルを解くために必要な
# > コンパートメントおよびそれに関連する移行係数が追加されている（付録Bを参照）。クロムの
# > モデルにおいて明示的に指定されていないコンパートメント内でクロムが生成される場合、
# > その移行速度は次のように仮定される。すなわち、血液コンパートメント内で生成される場合は
# > 1000 d⁻¹、骨体積コンパートメント内で生成される場合は骨代謝回転速度、その他の
# > コンパートメント内で生成される場合は0.25 d⁻¹とする。

  {Mn}/Plasma             Plasma1                1000
  {Mn}/RBC                Plasma1                1000

  {Mn}/C-bone-S           C-bone-S                  ---
  {Mn}/T-bone-S           T-bone-S                  ---
  {Mn}/C-bone-V           Plasma1               $( 3% / 365)
  {Mn}/T-bone-V           Plasma1               $(18% / 365)

  {Mn}/Brain              Plasma1                   0.25
  {Mn}/Liver1             Plasma1                   0.25
  {Mn}/Liver2             Plasma1                   0.25
  {Mn}/Kidneys            Kidneys                   0.25
  {Mn}/Pancreas           Plasma1                   0.25

  {Mn}/ST0                Plasma1                   0.25
  {Mn}/ST1                Plasma1                   0.25
  {Mn}/ST2                Plasma1                   0.25
