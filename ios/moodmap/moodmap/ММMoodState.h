//
//  ММMoodState.h
//  moodmap
//
//  Created by Roman Putsykovich on 10/27/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface __MoodState : NSObject

@property (strong, nonatomic) NSString *mood;
@property (strong, nonatomic) NSString *location;
@property (strong, nonatomic) NSString *moodImage;
@property (strong, nonatomic) NSString *timeAgo;

@end
