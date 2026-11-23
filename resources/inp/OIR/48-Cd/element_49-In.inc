[{In}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   Transferrin           Blood
  acc   Marrow1               R-marrow
  acc   Marrow2               R-marrow
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas      # for progeny of Cd
  acc   Muscle                Muscle        # for progeny of Cd
  acc   ST0                   Other
  acc   ST1                   Other


[{In}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.255 Table 27.3
  Plasma                  Transferrin              83
  Plasma                  RBC                       0.415
  RBC                     Plasma                    0.0554
  Transferrin             Marrow1                   0.316
  Transferrin             Liver1                    0.253
# Transferrin             ST0                       0.427       # fix
  Transferrin             ST1                       0.586
  Marrow1                 Transferrin               1.10
  Marrow1                 Marrow2                   0.475
  Marrow2                 Marrow1                   0.00831
  Liver1                  Transferrin               0.475
  Liver1                  SI-conRe                  0.110
  Liver1                  Liver2                    0.554
  Liver2                  Liver1                    0.00831
  ST0                     Plasma                    2.37
  ST1                     Plasma                    0.00475
  Plasma                  Kidneys                   1.66
  Kidneys                 Plasma                    0.0166
  Kidneys                 UB-con                    0.0268

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

  {Cd}/C-bone-S           Plasma                    2.37
  {Cd}/T-bone-S           Plasma                    2.37
  {Cd}/C-bone-V           Plasma                $( 3% / 365)
  {Cd}/T-bone-V           Plasma                $(18% / 365)

  {Cd}/Liver              Plasma                    2.37
  {Cd}/Kidneys            Kidneys                   ---
  {Cd}/Pancreas           Pancreas                  ---
  {Cd}/Muscle             Muscle                    ---

  {Cd}/ST0                Plasma                    2.37
  {Cd}/ST1                Plasma                    2.37

  Pancreas                Plasma                    2.37
  Muscle                  Plasma                    2.37
  Transferrin             Pancreas                  0.001       # 
  Transferrin             Muscle                    0.2         # 
  Transferrin             ST0                       0.226       # 質問書より
