[{W}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Exch-C-bone-V         C-bone-V
  acc   Noch-C-bone-V         C-bone-V
  acc   Exch-T-bone-V         T-bone-V
  acc   Noch-T-bone-V         T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Spleen                Spleen
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Other0                Other         # Rapid turnover
  acc   Other1                Other         # Intermediate turnover
  acc   Other2                Other         # Slow turnover
  exc   OtherExcreta          ---


[{W}:transfer]
#-----------------------+---------------------+--------------
# From                  | To                  | Coefficient[/d] or [%]
#-----------------------+---------------------+--------------

# ICRP Publ.151 p.284 Table 31.3
  Plasma                  Other0                    4.99
  Plasma                  RBC                       0.0582
  Plasma                  UB-con                    8.74
  Plasma                  Kidneys1                  0.524
  Plasma                  Kidneys2                  0.0582
  Plasma                  RC-con                    0.582
  Plasma                  Spleen                    0.00582
  Plasma                  Liver1                    0.466
  Plasma                  Other1                    0.262
  Plasma                  Other2                    0.0233
  Plasma                  T-bone-S                  0.518
  Plasma                  C-bone-S                  0.414
  Other0                  Plasma                    8.32
  RBC                     Plasma                    0.347
  Kidneys1                UB-con                    1.39
  Kidneys2                Plasma                    0.0019
  Liver1                  Plasma                    0.312
  Liver1                  Liver2                    0.0347
  Other1                  Plasma                    0.0693
  Other2                  OtherExcreta              0.0019
  Spleen                  Plasma                    0.0019
  T-bone-S                Plasma                    0.578
  T-bone-S                Exch-T-bone-V             0.116
  C-bone-S                Plasma                    0.578
  C-bone-S                Exch-C-bone-V             0.116
  Liver2                  Plasma                    0.0019
  Noch-T-bone-V           Plasma                    0.000493
  Noch-C-bone-V           Plasma                    0.0000821
  Exch-T-bone-V           T-bone-S                  0.00277
  Exch-T-bone-V           Noch-T-bone-V             0.00416
  Exch-C-bone-V           C-bone-S                  0.00277
  Exch-C-bone-V           Noch-C-bone-V             0.00416

# ICRP Publ.151 p.299 Para.588
# > (588) Progeny of osmium addressed in this publication are isotopes of osmium,
# > rhenium, tungsten, and iridium. The characteristic models for osmium and iridium
# > are applied without change to these elements as progeny of osmium. The models for
# > rhenium and tungsten as progeny of osmium are expansions of the characteristic
# > models for these elements used in this publication, with added compartments and
# > associated transfer coefficients needed to solve the linked biokinetic models for
# > chains headed by osmium (see Annex B). If produced in an ambiguous compartment
# > (i.e. a compartment not explicitly named in the model for rhenium or tungsten), the
# > progeny is assumed to transfer at a specified rate to the central blood compartment
# > of its characteristic biokinetic model, and to follow that model thereafter. The following
# > transfer rates to the central blood compartment are assigned to rhenium or
# > tungsten produced in an ambiguous compartment: 1000 d⁻¹ if produced in a blood
# > compartment; at the rate of bone turnover if produced in a bone volume
# > compartment; and at the following element-specific rates if produced in any other
# > ambiguous compartment: rhenium, 0.462 d⁻¹; and tungsten, 8.32 d⁻¹.
# > (588) 本刊行物で扱うオスミウムの子孫元素は、オスミウム、レニウム、タングステン、
# > およびイリジウムの同位体である。オスミウムとイリジウムの特性モデルは、オスミウムの
# > 子孫元素としてのこれらの元素にもそのまま適用される。オスミウムの子孫元素としての
# > レニウムとタングステンのモデルは、本刊行物で使用されているこれらの元素の特性モデルの
# > 拡張版であり、オスミウムを先頭とする連鎖の連結生体動態モデルを解くために必要な
# > コンパートメントと関連する移動係数が追加されている（付録B参照）。
# > あいまいなコンパートメント（すなわち、レニウムまたはタングステンのモデルで明示的に
# > 名前が付けられていないコンパートメント）で生成された場合、子孫元素は、
# > その特性生体動態モデルの中央血液コンパートメントに指定された速度で移行し、その後は
# > そのモデルに従うと仮定される。不明瞭なコンパートメントで生成されたレニウムまたは
# > タングステンの中心血液コンパートメントへの移行速度は、血液コンパートメントで
# > 生成された場合は1000 d⁻¹、骨量コンパートメントで生成された場合は骨代謝の速度、
# > その他の不明瞭なコンパートメントで生成された場合は、以下の元素固有の移行速度と
# > なります：レニウム：0.462 d⁻¹、タングステン：8.32 d⁻¹。

  {Os}/Blood1             Plasma                 1000
  {Os}/Blood2             Plasma                 1000
  {Re}/Blood              Plasma                 1000

  {*}/C-bone-S            C-bone-S                  ---
  {*}/T-bone-S            T-bone-S                  ---
  {*}/C-bone-V            Plasma                $( 3% / 365)
  {*}/T-bone-V            Plasma                $(18% / 365)

  {Re}/S-glands           Plasma                    8.32
  {Re}/OralcavityRe       Oralcavity                ---
  {Re}/Oesophagus-sRe     Oesophagus-s              ---
  {Re}/St-conRe           St-con                    ---
  {Re}/St-wall            Plasma                    8.32
  {Re}/RC-wall            Plasma                    8.32
  {Re}/Thyroid1           Plasma                    8.32
  {Re}/Thyroid2           Plasma                    8.32
  {*}/Liver1              Liver1                    ---
  {*}/Liver2              Liver2                    ---
  {*}/Kidneys1            Kidneys1                  ---
  {*}/Kidneys2            Kidneys2                  ---

  {Os}/ST0                Plasma                    8.32
  {Os}/ST1                Plasma                    8.32
  {Os}/ST2                Plasma                    8.32
  {Re}/ST0                Plasma                    8.32
  {Re}/ST1                Plasma                    8.32
  {Re}/ST2                Plasma                    8.32
