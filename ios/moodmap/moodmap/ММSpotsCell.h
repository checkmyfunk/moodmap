//
//  ММSpotsCell.h
//  moodmap
//
//  Created by Roman Putsykovich on 10/27/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface __SpotsCell : UITableViewCell

@property (weak, nonatomic) IBOutlet UILabel *placeLabel;
@property (weak, nonatomic) IBOutlet UILabel *locationLabel;
@property (weak, nonatomic) IBOutlet UILabel *distanceLabel;
@property (weak, nonatomic) IBOutlet UIImageView *spotsImage;

@end
