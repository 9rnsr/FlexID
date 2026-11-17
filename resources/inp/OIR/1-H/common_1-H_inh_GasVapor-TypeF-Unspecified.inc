[parameter]
  # ICRP Publ.134 p.21 Table 2.3 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 100
  $ss =   0
  $fb = 0
  $sb = 0

  ${H}/fA    = 0.99 # ≒1   # ICRP Publ.134 p.20 Table 2.2 / Unspecified, Absorption type F
  ${H}/fA_Re = 0

  $Blood = 'Blood'


[intake]
#---------------------+-------------------
# To                  | Transfer Coef.[%]
#---------------------+-------------------

# ICRP Publ.130 p.62 Para.92
# ICRP Publ.134 p.20 Table 2.2
  ET2-F                    19.96%       # = 99.8% of 20%
  ETseq-F                   0.04%       # =  0.2% of 20%
  BB-F                      9.98%       # = 99.8% of 10%
  BBseq-F                   0.02%       # =  0.2% of 10%
  bb-F                     19.96%       # = 99.8% of 20%
  bbseq-F                   0.04%       # =  0.2% of 20%
  ALV-F                    50.0%        # =          50%


[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_1-H_HTO.inc]
