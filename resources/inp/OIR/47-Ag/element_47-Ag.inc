[{Ag}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other


[{Ag}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.233 Table 25.3
  Blood1                  Blood2                    0.72
  Blood1                  Kidneys                   1.2
  Blood1                  Liver1                   30
  Blood1                  T-bone-S                  1.2
  Blood1                  C-bone-S                  1.2
  Blood1                  ST0                      12.8
  Blood1                  ST1                      12.8
  Blood1                  UB-con                    0.13
  Blood2                  Blood1                    0.231
  Kidneys                 Blood1                    0.8
  Liver1                  SI-conRe                  1.0
  Liver1                  Liver2                    9.0
  Liver2                  Blood1                    0.2
  ST0                     Blood1                    8.0
  ST1                     Blood1                    0.4
  T-bone-S                Blood1                    0.07
  C-bone-S                Blood1                    0.07

$if progeny

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

  {Ag}/Blood1             Blood1                    ---
  {Ag}/Blood2             Blood2                    ---

  {Ag}/C-bone-S           C-bone-S                  ---
  {Ag}/T-bone-S           T-bone-S                  ---

  {Ag}/Liver1             Liver1                    ---
  {Ag}/Liver2             Liver2                    ---
  {Ag}/Kidneys            Kidneys                   ---

  {Ag}/ST0                ST0                       ---
  {Ag}/ST1                ST1                       ---

$endif
