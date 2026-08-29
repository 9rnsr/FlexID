[{Xe}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  exc   Exhaled               ---


[{Xe}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

$if progeny

# ICRP Publ.137 Para.252
# > (252) A generic biokinetic model is applied in this publication to xenon isotopes
# > produced by decay of radionuclides in systemic pools. Xenon produced in bone is
# > assumed to transfer to blood at the rate 100 d⁻¹ if produced in bone surfaces and
# > 0.36 d⁻¹ if produced in bone volume. These rates are taken from the model for radon
# > introduced in Publication 67 (ICRP, 1993). Xenon produced in a soft tissue compartment
# > is assumed to transfer to blood with a half-time of 20 min. Xenon produced
# > in the blood inorganic iodide compartment is assigned to the blood compartment of
# > the xenon model. Xenon produced in the blood organic iodine compartment is
# > assumed to transfer to blood in the xenon model at the rate 1000 d⁻¹. Xenon entering
# > the blood compartment of the xenon model or produced in that compartment is
# > assumed to be exhaled at the rate 1000 d⁻¹.
# > (252) 本刊行物では、全身プールにおける放射性核種の崩壊によって生成されるキセノン同位体に
# > 対し、一般的な生体動態モデルを適用する。骨で生成されるキセノンは、骨表面で生成される場合は
# > 100 d⁻¹の速度で血液に移行すると仮定し、骨体積で生成される場合は0.36 d⁻¹の速度で血液に
# > 移行すると仮定する。これらの速度は、Publication 67 (ICRP, 1993) で導入されたラドンの
# > モデルに基づく。軟部組織コンパートメントで生成されるキセノンは、半減期20分で血液に
# > 移行すると仮定する。血液中の無機ヨウ素コンパートメントで生成されるキセノンは、
# > キセノンモデルの血液コンパートメントに割り当てられる。血液中の有機ヨウ素コンパートメントで
# > 生成されるキセノンは、キセノンモデルにおいて1000 d⁻¹の速度で血液に移行すると仮定する。
# > キセノンモデルの血液コンパートメントに入る、またはそこで生成されるキセノンは、1000 d⁻¹の
# > 速度で呼気中に排出されると仮定されます。

  {I}/Blood1              Blood                     ---
  {I}/Blood2              Blood                  1000
  {Xe}/Blood              Blood                     ---

  {I}/S-glands            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/OralcavityRe        Oralcavity                ---
  {I}/Oesophagus-sRe      Oesophagus-s              ---
  {I}/St-conRe            St-con                    ---
  {I}/St-wall             Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Thyroid1            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Thyroid2            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Liver1              Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Liver2              Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Kidneys1            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Kidneys2            Blood                 $(0.693 / (20/60/24))   # 半減期20分

  {I}/Other1              Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Other2              Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Other3              Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Other4              Blood                 $(0.693 / (20/60/24))   # 半減期20分

  {Xe}/Exhaled            Exhaled                   ---

$endif

  Blood                   Exhaled                1000
