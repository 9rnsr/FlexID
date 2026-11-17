[parameter]
  # ICRP Publ.134 p.53 Table 3.3 / Methane, Absorption type V

  ${C}/fA    = 0            # 小腸から血液への吸収は無い
  ${C}/fA_Re = 0            # 小腸から血液への吸収は無い

  ${C}/Blood = 'BloodM'     # Methane model


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.134 p.52 Table 3.2
# ICRP Publ.134 p.54 Para.88
  $Blood                   0.3%
  Environment             99.7%


[{C}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  exc   Environment           ---


[include:../share/common_HATM+UB.inc]

[include:element_6-C_Methane.inc]
