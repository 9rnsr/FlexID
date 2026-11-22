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

# ICRP Publ.151 p.233 Pra.466
# > (466) Progeny of silver addressed in this publication are isotopes of silver, rhodium,
# > palladium, indium, and cadmium. The model for silver as a parent is applied
# > to silver produced by decay of another isotope of silver. The models for rhodium,
# > palladium, indium, and cadmium as progeny of silver are the characteristic models
# > for these elements with added compartments and associated transfer coefficients
# > needed to solve the linked biokinetic models for chains headed by silver. Muscle
# > and pancreas were added to the explicitly identified tissues in the characteristic model
# > for indium. The following rates of transfer of indium between blood compartments
# > in the characteristic model for indium and the added tissues were assigned: transferrin
# > to pancreas, 0.001 d⁻¹; transferrin to muscle, 0.2 d⁻¹; pancreas to plasma,
# > 2.37 d⁻¹; and muscle to plasma, 2.37 d⁻¹. Rhodium, palladium, indium, or cadmium
# > produced in a compartment of the model for a preceding chain that is not a compartment
# > in the model for that progeny (an ambiguous compartment) is assumed to
# > transfer to the central blood compartment of the progeny’s model, and to follow that
# > model thereafter. The following transfer rates are assigned to progeny produced in
# > ambiguous compartments: 1000 d⁻¹ if produced in a blood compartment; at the rate
# > of bone turnover for the indicated bone type if produced in a bone volume
# > compartment; and at the following element-specific rates if produced in any other
# > compartment: rhodium, 0.09902 d⁻¹; palladium, 0.1386 d⁻¹; indium, 2.37 d⁻¹; and
# > cadmium, 0.5 d⁻¹.
# > (466) 本出版物で扱う銀の娘核種には、銀、ロジウム、パラジウム、インジウム、および
# > カドミウムの同位体が含まれます。親核種としての銀に関するモデルは、別の銀同位体の
# > 崩壊によって生成される銀にも適用されます。銀の娘核種であるロジウム、パラジウム、
# > インジウム、およびカドミウムに関するモデルは、各元素固有のモデルをベースとしつつ、
# > 銀を起点とする崩壊系列の連鎖的な生物学的動態モデルを解くために必要なコンパートメント
# > および関連する移行係数を追加したものです。インジウムの固有モデルにおいて明示的に
# > 特定された組織には、筋肉および膵臓が追加されました。インジウムの固有モデルにおける
# > 血液コンパートメントと、追加された組織との間におけるインジウムの移行速度は、
# > トランスフェリンから膵臓へ0.001 d⁻¹、トランスフェリンから筋肉へ0.2 d⁻¹、膵臓から血漿へ
# > 2.37 d⁻¹、筋肉から血漿へ2.37 d⁻¹と設定されました。崩壊系列の先行段階のモデルにおいて
# > 生成されたロジウム、パラジウム、インジウム、またはカドミウムが、その娘核種のモデル内には
# > 存在しないコンパートメント（「曖昧なコンパートメント」と呼ぶ）で生成された場合、
# > それらは当該娘核種のモデルにおける中心的な血液コンパートメントへ移行し、その後は
# > そのモデルに従うものと仮定されます。曖昧なコンパートメントで生成された娘核種には、
# > 以下の移行速度が割り当てられます：血液コンパートメントで生成された場合は1000 d⁻¹、
# > 骨体積コンパートメントで生成された場合は当該骨種における骨代謝回転速度、
# > その他のコンパートメントで生成された場合は各元素固有の速度（ロジウム：0.09902 d⁻¹、
# > パラジウム：0.1386 d⁻¹、インジウム：2.37 d⁻¹、カドミウム：0.5 d⁻¹）。

  {Ag}/Blood1             Plasma                 1000
  {Ag}/Blood2             Plasma                 1000
  {Cd}/Plasma             Plasma                    ---
  {Cd}/RBC                RBC                       ---

  {Ag,Cd}/C-bone-S        C-bone-S                  ---
  {Ag,Cd}/T-bone-S        T-bone-S                  ---
  {Cd}/C-bone-V           C-bone-V                  ---
  {Cd}/T-bone-V           T-bone-V                  ---

  {Ag}/Liver1             Plasma                    0.5
  {Ag}/Liver2             Plasma                    0.5
  {Cd}/Liver              Liver                     ---
  {Ag,Cd}/Kidneys         Kidneys                   0.5
  {Cd}/Pancreas           Pancreas                  ---
  {Cd}/Muscle             Muscle                    ---

  {Ag}/ST0                Plasma                    0.5
  {Ag}/ST1                Plasma                    0.5
  {Cd}/ST0                ST0                       ---
  {Cd}/ST1                ST1                       ---
