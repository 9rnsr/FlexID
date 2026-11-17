[{C}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood         # CO model
  exc   Exhalation            ---


[{C}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.64 Para.122-123
# > (122) Inhaled carbon monoxide diffuses readily across the membranes of the AI
# > region of the lung and enters the pulmonary blood, where it is bound to haemoglobin
# > (ICRP, 1987). It is released from haemoglobin and removed from the body in expired
# > air over a period of hours.
# > (123) The model for inhaled carbon monoxide used in this publication is the model
# > applied in Publication 30 (ICRP, 1981) and Publication 71 (ICRP, 1995). It is
# > assumed that 40% of inhaled carbon monoxide is absorbed instantly into blood
# > and bound to haemoglobin, and 60% is exhaled instantly. Carbon monoxide is
# > assumed to be lost from blood to the environment via the lungs with a biological
# > half-time of 200 min (Glass et al., 1968; Peterson and Stewart, 1970).
# > (122) 吸入された一酸化炭素は、肺のAI領域の膜を容易に拡散して肺血流に入り、そこで
# > ヘモグロビンと結合する（ICRP, 1987）。その後、ヘモグロビンから解離し、数時間をかけて
# > 呼気とともに体外へ排出される。
# > (123) 本報告書で用いる吸入一酸化炭素のモデルは、Publication 30（ICRP, 1981）および
# > Publication 71（ICRP, 1995）で適用されたモデルである。吸入された一酸化炭素のうち
# > 40%は直ちに血液中に吸収されてヘモグロビンと結合し、60%は直ちに呼気として排出されると
# > 仮定する。また、血液中の一酸化炭素は、肺を介して環境中へ消失し、その生物学的半減期は
# > 200分であると仮定する（Glass et al., 1968; Peterson and Stewart, 1970）。

  Blood                   Exhalation            $(0.693 / (200/60/24))      # 半減期200分
