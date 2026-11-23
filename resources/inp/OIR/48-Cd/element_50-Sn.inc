[{Sn}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   RedMarrow             R-marrow      # for progeny of Cd
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Pancreas              Pancreas      # for progeny of Cd
  acc   Muscle                Muscle        # for progeny of Cd
  acc   Other1                Other
  acc   Other2                Other


[{Sn}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.255 Table 27.3
  Blood                   UB-con                    1.8
  Blood                   RC-con                    0.2
  Blood                   T-bone-S                  0.6
  Blood                   C-bone-S                  0.6
# Blood                   Other1                    0.6     # fix
  Blood                   Other2                    1.0
  Blood                   Liver1                    0.075
  Blood                   Liver2                    0.025
  Blood                   Kidneys1                  0.05
  Blood                   Kidneys2                  0.05
  T-bone-S                Blood                     0.035
  C-bone-S                Blood                     0.035
  T-bone-S                T-bone-V                  0.035
  C-bone-S                C-bone-V                  0.035
  T-bone-V                Blood                     0.0035
  C-bone-V                Blood                     0.0035
  Liver1                  Blood                     0.0116
  Liver2                  Blood                     0.00077
  Kidneys1                UB-con                    0.139
  Kidneys2                Blood                     0.0116
  Other1                  Blood                     0.139
  Other2                  Blood                     0.0035

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

  {Cd,In}/Plasma          Blood                     ---
  {Cd,In}/RBC             Blood                  1000
  {In}/Transferrin        Blood                  1000

  {Cd}/C-bone-S           C-bone-S                  ---
  {Cd}/T-bone-S           T-bone-S                  ---
  {Cd}/C-bone-V           C-bone-V                  ---
  {Cd}/T-bone-V           T-bone-V                  ---

  {In}/Marrow1            Blood                     0.139
  {In}/Marrow2            Blood                     0.139
  {Cd}/Liver              Blood                     0.139
  {In}/Liver1             Blood                     0.139
  {In}/Liver2             Blood                     0.139
  {Cd,In}/Kidneys         Blood                     0.139
  {Cd,In}/Pancreas        Pancreas                  ---
  {Cd,In}/Muscle          Muscle                    ---

  {Cd}/ST0                Blood                     0.139
  {Cd}/ST1                Blood                     0.139
  {In}/ST0                Blood                     0.139
  {In}/ST1                Blood                     0.139

  RedMarrow               Blood                     0.139
  Pancreas                Blood                     0.139
  Muscle                  Blood                     0.139
  Blood                   RedMarrow                 0.012
  Blood                   Pancreas                  0.0012
  Blood                   Muscle                    0.297
  Blood                   Other1                    0.2898      # 質問書より
