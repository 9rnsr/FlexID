[{Ar}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  exc   Exhaled               ---


[{Ar}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.75 Para.135
# > (135) Progeny of chlorine addressed in this publication are radioisotopes of chlorine
# > and argon. The model for chlorine as a parent is assigned to chlorine as a progeny
# > of chlorine. Argon produced in a tissue (i.e. in ‘Other’) is assumed to transfer to
# > blood with a half-time of 15 min, and from blood to the environment (via exhalation)
# > at a rate of 1000 d⁻¹.
# > (135) 本書で扱う塩素の生成核種は、塩素およびアルゴンの放射性同位体である。親核種としての
# > 塩素に適用されるモデルは、生成核種としての塩素にも同様に適用される。組織内（すなわち
# > 「その他」の部位）で生成されたアルゴンは、15分の半減期で血液へ移行し、さらに血液から
# > （呼気を通じて）環境中へ、1000 d⁻¹の速度で移行すると仮定する。

  {Cl}/Blood              Blood                     ---
  {Cl}/Other              Blood                 $(0.693/(15/60/24))

  Blood                   Exhaled                1000
