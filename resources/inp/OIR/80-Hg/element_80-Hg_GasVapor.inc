[{Hg}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma0               Blood     # Gas,Vapor only
  acc   Plasma1               Blood
  acc   Plasma2               Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Brain1                Brain
  acc   Brain2                Brain
  acc   Other1                Other
  acc   Other2                Other
  exc   Excreta               ---


[{Hg}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.338 Table 36.4
  Plasma0                 RBC                     100
  Plasma0                 Brain1                   20
  Plasma0                 Kidneys                 100
  Plasma0                 Liver                    60
  Plasma0                 Other1                  650
  Plasma0                 Excreta                  70
  Plasma1                 RBC                       0.48
  Plasma1                 Plasma2                   2.4
  Plasma1                 Kidneys                   7.2
  Plasma1                 Liver                     4.8
  Plasma1                 Brain1                    0.048
  Plasma1                 T-bone-S                  0.024
  Plasma1                 C-bone-S                  0.024
  Plasma1                 Other1                    5.184
  Plasma1                 Other2                    0.72
  Plasma1                 SI-conRe                  1.92
  Plasma1                 Excreta                   1.2
  RBC                     Plasma1                   0.33
  Plasma2                 Plasma1                   0.6
  Kidneys                 UB-con                    0.0198
  Liver                   SI-conRe                  0.1733
  Brain1                  Plasma1                   0.0329
  Brain1                  Brain2                    0.00173
  Brain2                  Plasma1                   0.00038
  T-bone-S                Plasma1                   0.0347
  C-bone-S                Plasma1                   0.0347
  Other1                  Plasma1                   0.0347
  Other2                  Plasma1                   0.00693

$if progeny

# ICRP Publ.151 p.337 Para.700
# > (700) Progeny of mercury addressed in this publication are radioisotopes of mercury,
# > gold, osmium, and platinum. The models for all four elements as progeny of mercury
# > are expansions of the characteristic models for these elements with added compartments
# > and associated transfer coefficients needed to solve the linked biokinetic models
# > for chains headed by mercury (see Annex B). If produced in an ambiguous compartment
# > (i.e. a compartment not explicitly named in the progeny’s model), the progeny is
# > assumed to transfer at a specified rate to the central blood compartment of its characteristic
# > biokinetic model, and to follow that model thereafter. The following transfer
# > rates to the central blood compartment are assigned to mercury, gold, osmium, and
# > platinum produced in an ambiguous compartment: 1000 d⁻¹ if produced in a blood
# > compartment; and at the following element-specific rates if produced in any other
# > ambiguous compartment: gold, 0.0693 d⁻¹; and osmium or platinum, 0.09902 d⁻¹.
# > (700) 本書で扱う水銀の娘核種は、水銀、金、オスミウム、および白金の放射性同位体です。
# > 水銀の娘核種としてのこれら4元素のモデルは、各元素固有のモデルを拡張したものです。
# > 具体的には、水銀を起点とする崩壊系列について、連鎖的な生物学的動態モデルを解くために
# > 必要なコンパートメントおよびそれに関連する移行係数が追加されています（付録Bを参照）。
# > 娘核種が「不明確なコンパートメント」（すなわち、その娘核種のモデルにおいて明示的に
# > 名称が定義されていないコンパートメント）で生成された場合、その娘核種は、所定の移行速度で
# > 当該元素固有の生物学的動態モデルにおける「中心血液コンパートメント」へ移行し、その後は
# > そのモデルに従うものと仮定されます。不明確なコンパートメントで生成された水銀、金、
# > オスミウム、および白金について、中心血液コンパートメントへの移行速度は次のように
# > 設定されます：血液コンパートメント内で生成された場合は1000 d⁻¹とし、それ以外の
# > 不明確なコンパートメントで生成された場合は、元素ごとに以下の移行速度を適用します：
# > 金については0.0693 d⁻¹、オスミウムまたは白金については0.09902 d⁻¹。

  {Hg}/Plasma0            Plasma0                   ---     # Gas,Vapor only
  {Hg}/Plasma1            Plasma1                   ---
  {Hg}/Plasma2            Plasma2                   ---
  {Hg}/RBC                RBC                       ---

  {Hg}/C-bone-S           C-bone-S                  ---
  {Hg}/T-bone-S           T-bone-S                  ---

  {Hg}/Liver              Liver                     ---
  {Hg}/Kidneys            Kidneys                   ---
  {Hg}/Brain1             Brain1                    ---
  {Hg}/Brain2             Brain2                    ---

  {Hg}/Other1             Other1                    ---
  {Hg}/Other2             Other2                    ---

  {Hg}/Excreta            Excreta                   ---

$endif
