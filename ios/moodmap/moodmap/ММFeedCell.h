//
//  ММFeedCell.h
//  moodmap
//
//  Created by Roman Putsykovich on 10/26/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface __FeedCell : UITableViewCell
@property (weak, nonatomic) IBOutlet FBProfilePictureView *profilePicture;
@property (weak, nonatomic) IBOutlet UILabel *nameLabel;
@property (weak, nonatomic) IBOutlet UILabel *locationLabel;
@property (weak, nonatomic) IBOutlet UILabel *moodLabel;

@end
