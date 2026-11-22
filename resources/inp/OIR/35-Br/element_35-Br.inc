[{Br}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   Other                 Other


[{Br}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.199 Table 21.4
  Blood                   Other                   200
  Blood                   UB-con                    0.83
  Blood                   RC-con                    0.0083
  Other                   Blood                    15

$if progeny

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

  {Br}/Blood              Blood                     ---
  {Br}/Other              Other                     ---

$endif
