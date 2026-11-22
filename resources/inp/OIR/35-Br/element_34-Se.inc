[{Se}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   Spleen                Spleen
  acc   Testes                Testes
  acc   Ovaries               Ovaries
  acc   ST1                   Other
  acc   ST2                   Other


[{Se}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.192 Table 20.3
  Plasma                  Liver                     1.2
  Plasma                  Kidneys                   0.6
  Plasma                  Pancreas                  0.08
  Plasma                  Spleen                    0.11
  Plasma                  Testes                    0.04
  Plasma                  Ovaries                   0.013
  Plasma                  ST1                       5.2
  Plasma                  ST2                       0.5
  Plasma                  UB-con                    1.0
  Plasma                  RC-con                    0.4
  Plasma                  T-bone-S                  0.2
  Plasma                  C-bone-S                  0.2
  Plasma                  RBC                       0.5
  Liver                   Plasma                    0.08
  Kidneys                 Plasma                    0.08
  Spleen                  Plasma                    0.08
  Pancreas                Plasma                    0.08
  Testes                  Plasma                    0.08
  Ovaries                 Plasma                    0.08
  ST1                     Plasma                    0.08
  ST2                     Plasma                    0.005
  RBC                     Plasma                    0.035
  T-bone-S                Plasma                    0.015
  C-bone-S                Plasma                    0.015

# ICRP Publ.151 p.199 Para.397
# > (397) Progeny of bromine addressed in this publication are radioisotopes of bromine,
# > krypton, and selenium. The model for bromine as a parent is assigned to bromine
# > as a progeny. Krypton produced in tissues is assumed to transfer to blood with a
# > half-time of 15 min and from blood to the environment (via exhalation) at a rate of
# > 1000 d⁻¹. Selenium produced in a tissue is assumed to transfer to blood at a rate of
# > 0.08 d⁻¹ and then to follow the characteristic model for selenium.
# > (397) 本書で扱う臭素の娘核種は、臭素、クリプトン、およびセレンの放射性同位体です。
# > 親核種としての臭素に適用されるモデルは、娘核種としての臭素にも適用されます。組織内で
# > 生成されたクリプトンは、15分の半減期で血液へ移行し、その後（呼気を通じて）血液から
# > 環境中へ1000 d⁻¹の速度で移行すると仮定されます。組織内で生成されたセレンは、0.08 d⁻¹の
# > 速度で血液へ移行し、その後はセレン固有のモデルに従うものと仮定されます。

  {Br}/Blood              Plasma                    ---
  {Br}/Other              Plasma                    0.08
