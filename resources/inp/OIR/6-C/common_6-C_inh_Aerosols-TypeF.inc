[parameter]
  # ICRP Publ.134 p.53 Table 3.3 / Inhaled particulate materials, Absorption type F
  $fr = 1
  $sr = 100
  $ss =   0
  $fb = 0                   # footnote†
  $sb = 0                   # footnote†

  ${C}/fA    = 0.99 # ≒1   # ICRP Publ.134 p.53 Table 3.3 / Inhaled particulate materials, Absorption type F
  ${C}/fA_Re = 0.99 # ≒1   # ICRP Publ.134 p.53 Table 3.3 / footnote††

  ${C}/Blood = 'Blood0'     # Generic model


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_6-C_Generic.inc]
