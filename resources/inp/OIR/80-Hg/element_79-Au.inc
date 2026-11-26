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


[{Au}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.321 Table 35.3
  Blood1                  Blood2                    0.1
  Blood1                  Kidneys                   0.1
  Blood1                  Liver                     0.1
  Blood1                  Other1                    0.18
  Blood1                  Other2                    0.1
  Blood1                  UB-con                    0.3
  Blood1                  RC-con                    0.1
  Blood1                  T-bone-S                  0.01
  Blood1                  C-bone-S                  0.01
  Blood2                  Blood1                    0.139
  Kidneys                 UB-con                    0.0693
  Liver                   Blood1                    0.0693
  Other1                  Blood1                    0.0693
  Other2                  Blood1                    0.0139
  T-bone-S                Blood1                    0.0693
  C-bone-S                Blood1                    0.0693

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

  {Hg}/Plasma0            Blood1                 1000       # Gas,Vapor only
  {Hg}/Plasma1            Blood1                    ---
  {Hg}/Plasma2            Blood1                 1000
  {Hg}/RBC                Blood1                 1000
  {Au}/Blood1             Blood1                    ---
  {Au}/Blood2             Blood2                    ---

  {Hg,Au}/C-bone-S        C-bone-S                  ---
  {Hg,Au}/T-bone-S        T-bone-S                  ---

  {Hg,Au}/Liver           Liver                     ---
  {Hg,Au}/Kidneys         Kidneys                   ---
  {Hg}/Brain1             Blood1                    0.0693
  {Hg}/Brain2             Blood1                    0.0693

  {Hg}/Other1             Blood1                    0.0693
  {Hg}/Other2             Blood1                    0.0693
  {Au}/Other1             Other1                    ---
  {Au}/Other2             Other2                    ---
