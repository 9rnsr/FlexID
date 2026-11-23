[{Cd}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   Muscle                Muscle
  acc   ST0                   Other
  acc   ST1                   Other


[{Cd}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.246 Table 26.3
  Plasma                  Liver                   180
  Plasma                  Kidneys                  12
  Plasma                  Pancreas                  9.00
  Plasma                  Muscle                    6.0
  Plasma                  RBC                       0.1
  Plasma                  ST0                     120
  Plasma                  ST1                      94.5
  Plasma                  UB-con                    1.5
  Plasma                  RC-con                    1.5
  Plasma                  T-bone-S                  0.45
  Plasma                  C-bone-S                  0.9
  Liver                   Plasma                    0.018
  Kidneys                 Plasma                    0.0008
  Pancreas                Plasma                    0.018
  Muscle                  Plasma                    0.0011
  RBC                     Plasma                    0.00833
  ST0                     Plasma                    0.5
  ST1                     Plasma                    0.017
  T-bone-S                Plasma                    0.0002
  C-bone-S                Plasma                    0.0002
  T-bone-S                T-bone-V                  0.00001
  C-bone-S                T-bone-V                  0.00001
  T-bone-V                Plasma                    0.000493
  C-bone-V                Plasma                    0.0000821

$if progeny

# ICRP Publ.151 p.247 Para.508
# > (508) Progeny of cadmium addressed in this publication are radioisotopes of cadmium,
# > tin, indium, and silver. The model for cadmium as a parent is applied to cadmium
# > as a progeny of cadmium. The models for tin, indium, and silver as cadmium
# > progeny are expansions of the characteristic models for these elements with added
# > compartments and associated transfer coefficients needed to solve the linked biokinetic
# > models for chains headed by cadmium (see Annex B). The following transfer rates to
# > the central blood compartment in the model for tin, indium, or silver are assigned to
# > these progeny when produced in a compartment of a model for a preceding chain
# > member that is not contained in the model for tin, indium, or silver: 1000 d⁻¹ if
# > produced in a blood compartment; at the rate of bone turnover if produced in a
# > bone volume compartment; and at the following element-specific rates if produced in
# > any other compartment: tin, 0.139 d⁻¹; indium, 2.37 d⁻¹; and silver, 0.4 d⁻¹. Other
# > transfers added to the characteristic models for progeny are as follows: for tin, blood
# > to muscle=0.297 d⁻¹, blood to pancreas=0.0012 d⁻¹, blood to red marrow
# > =0.012 d⁻¹; for indium, blood to muscle=0.2 d⁻¹, blood to pancreas=0.001 d⁻¹;
# > for silver, blood to muscle=5.8 d⁻¹, blood to pancreas=0.028 d⁻¹.
# > (508) 本書で扱うカドミウムの娘核種は、カドミウム、スズ、インジウム、および銀の
# > 放射性同位体です。親核種としてのカドミウムに適用されるモデルは、カドミウムの
# > 娘核種であるカドミウムに対しても適用されます。カドミウムの娘核種であるスズ、インジウム、
# > および銀に関するモデルは、それぞれの元素の基本モデルを拡張したものです。これには、
# > カドミウムを起点とする崩壊系列の連鎖的な生物動態モデルを解くために必要な、コンパートメントの
# > 追加およびそれに関連する移行係数が含まれています（付録Bを参照）。スズ、インジウム、または
# > 銀のモデルにおいて、それらの娘核種が、当該元素のモデルには含まれていない先行する
# > 崩壊系列メンバーのモデル内のコンパートメントで生成される場合、中心血液コンパートメントへの
# > 移行速度は次のように設定されます。すなわち、血液コンパートメントで生成される場合は
# > 1000 d⁻¹、骨体積コンパートメントで生成される場合は骨代謝回転速度、その他のコンパートメントで
# > 生成される場合は、元素ごとにそれぞれ、スズ：0.139 d⁻¹、インジウム：2.37 d⁻¹、銀：0.4 d⁻¹ 
# > です。娘核種の基本モデルに追加されるその他の移行は以下の通りです。スズについては、
# > 血液から筋肉へ＝0.297 d⁻¹、血液から膵臓へ＝0.0012 d⁻¹、血液から赤色骨髄へ＝0.012 d⁻¹。
# > インジウムについては、血液から筋肉へ＝0.2 d⁻¹、血液から膵臓へ＝0.001 d⁻¹。銀については、
# > 血液から筋肉へ＝5.8 d⁻¹、血液から膵臓へ＝0.028 d⁻¹。

  {Cd}/Plasma             Plasma                    ---
  {Cd}/RBC                RBC                       ---

  {Cd}/C-bone-S           C-bone-S                  ---
  {Cd}/T-bone-S           T-bone-S                  ---
  {Cd}/C-bone-V           C-bone-V                  ---
  {Cd}/T-bone-V           T-bone-V                  ---

  {Cd}/Liver              Liver                     ---
  {Cd}/Kidneys            Kidneys                   ---
  {Cd}/Pancreas           Pancreas                  ---
  {Cd}/Muscle             Muscle                    ---

  {Cd}/ST0                ST0                       ---
  {Cd}/ST1                ST1                       ---

$endif
