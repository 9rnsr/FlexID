[parameter]
  # ICRP Publ.134 p.53 Table 3.3 / Inhaled particulate materials, Barium carbonate
  $fr = 1
  $sr = 100
  $ss =   0
  $fb = 0                   # footnote†
  $sb = 0                   # footnote†

  ${C}/fA    = 0.99 # ≒1   # ICRP Publ.134 p.53 Table 3.3 / Inhaled particulate materials, Barium carbonate
  ${C}/fA_Re = 0.99 # ≒1   # ICRP Publ.134 p.53 Table 3.3 / footnote††

  ${C}/Blood = 'Blood1'     # CO2 model


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_6-C_CO2.inc]
