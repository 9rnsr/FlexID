# ICRP Publ.137 p.84 Para.179
# > (179) A generic biokinetic model is applied in the OIR series to xenon isotopes
# > produced by decay of a radionuclide in systemic compartments. ...
# > (179) OIRシリーズでは、放射性核種の崩壊によって全身コンパートメントで生成される
# > キセノン同位体に対して、一般的な生体速度論モデルが適用されています。 ...

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

# ICRP Publ.137 p.84 Para.179
# > (179) ... Xenon produced in
# > bone is assumed to transfer to blood at the rate 100 d⁻¹ if produced in bone surfaces,
# > and 0.36 d⁻¹ if produced in bone volume. These rates are taken from the model for
# > radon introduced in Publication 67 (ICRP, 1993), and applied in the OIR series to
# > radon produced in bone surfaces and non-exchangeable bone volume, respectively,
# > by decay of a radium isotope. Xenon produced in a soft tissue compartment is
# > assumed to transfer to blood with a half-time of 20 min. Xenon produced in the
# > central blood compartment in the model for tellurium, antimony, or iodine is
# > assigned to the blood compartment of the xenon model. Xenon produced in any
# > other blood compartment in the tellurium, antimony, or iodine model is assumed to
# > transfer to blood in the xenon model at the rate 1000 d⁻¹. Xenon entering the blood
# > compartment of the xenon model or produced in that compartment is assumed to be
# > removed from the body (exhaled) at the rate 1000 d⁻¹. Partial recycling of xenon to
# > tissues via arterial blood is not depicted explicitly in this model for xenon as a
# > progeny radionuclide, but is considered in the assignment of the half-times in tissues.
# > The model is intended to yield a conservative average residence time of xenon atoms
# > in the body after their production in systemic pools.
# > (179) ... 骨で生成される
# > キセノンは、骨表面で生成される場合は100 d⁻¹の速度で血液に、骨体積で生成される場合は
# > 0.36 d⁻¹の速度で血液に移行すると仮定されます。これらの速度は、Publication 67 (ICRP, 1993)で
# > 導入されたラドンのモデルから採用されており、OIRシリーズでは、ラジウム同位体の崩壊によって
# > それぞれ骨表面および非交換性骨体積で生成されるラドンに適用されています。軟部組織
# > コンパートメントで生成されるキセノンは、半減期20分で血液に移行すると仮定される。
# > テルル、アンチモン、またはヨウ素のモデルにおいて中心血液コンパートメントで生成される
# > キセノンは、キセノンモデルの血液コンパートメントに割り当てられる。テルル、アンチモン、
# > またはヨウ素モデルにおける他の血液コンパートメントで生成されたキセノンは、
# > キセノンモデルにおいて1000 d⁻¹の速度で血液に移行すると仮定されます。キセノンモデルの
# > 血液コンパートメントに入るキセノン、またはそのコンパートメントで生成されるキセノンは、
# > 1000 d⁻¹の速度で体外に排出（呼気）されると仮定されます。動脈血を介したキセノンの
# > 部分的な組織への再循環は、子孫放射性核種としてのキセノンのこのモデルでは明示的に
# > 示されていませんが、組織における半減期の割り当てにおいては考慮されています。
# > このモデルは、全身プールで生成された後のキセノン原子の体内における平均滞留時間を
# > 保守的に算出することを目的としています。

  {Te,I}/Blood1           Blood                     ---
  {Xe}/Blood              Blood                     ---
  {Te}/Blood2             Blood                  1000
  {I}/Blood2              Blood                  1000

  {Te}/C-bone-S           Blood                   100
  {Te}/T-bone-S           Blood                   100
  {Te}/C-bone-V           Blood                     0.36
  {Te}/T-bone-V           Blood                     0.36

  {I}/S-glands            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/OralcavityRe        Oralcavity                ---
  {I}/Oesophagus-sRe      Oesophagus-s              ---
  {I}/St-conRe            St-con                    ---
  {I}/St-wall             Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {Te}/Thyroid            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Thyroid1            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Thyroid2            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {Te}/Liver              Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Liver1              Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Liver2              Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {Te}/Kidneys            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Kidneys1            Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Kidneys2            Blood                 $(0.693 / (20/60/24))   # 半減期20分

  {Te}/ST                 Blood                 $(0.693 / (20/60/24))   # 半減期20分
  {I}/Other1              Blood                  1000
  {I}/Other2              Blood                  1000

  {Xe}/Exhaled            Exhaled                   ---

$endif

  Blood                   Exhaled                1000
