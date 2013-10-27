//
//  ММHappySpotsViewController.m
//  moodmap
//
//  Created by Roman Putsykovich on 10/27/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import "ММHappySpotsViewController.h"
#import "ММSpotsCell.h"
#import "ММSpot.h"

@interface __HappySpotsViewController ()

@property (strong, nonatomic) NSArray *spots;

@end

@implementation __HappySpotsViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    self.spots = [self getSpots];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [self.spots count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    __Spot *spot =[self.spots objectAtIndex:indexPath.row];
    __SpotsCell *cell = [tableView dequeueReusableCellWithIdentifier:spot.spotIdentifier forIndexPath:indexPath];
    
    cell.placeLabel.text = spot.place;
    cell.locationLabel.text = spot.location;
    cell.distanceLabel.text = spot.distance;
    cell.imageView.image = [UIImage imageNamed:spot.imageName];
    
    return cell;
}

- (NSArray *)getSpots{
    
    NSMutableArray *array = [[NSMutableArray alloc] init];
    
    __Spot *spot = [[__Spot alloc] init];
    spot.place = @"Disrupt Europe - 100%";
    spot.location = @"Berlin, Germany";
    spot.imageName = @"chocolatefactory.png";
    spot.distance = @"0.0 km";
    spot.spotIdentifier = @"HappySpots5Cell";
    [array addObject:spot];
    
    spot = [[__Spot alloc] init];
    spot.place = @"Far Far Away - 98%";
    spot.location = @"Berlin, Germany";
    spot.imageName = @"disney.png";
    spot.distance = @"100.0 km";
    spot.spotIdentifier = @"HappySpots4Cell";
    [array addObject:spot];
    
    spot = [[__Spot alloc] init];
    spot.place = @"Chocolate Factory - 82%";
    spot.location = @"Berlin, Germany";
    spot.imageName = @"disrupt.png";
    spot.distance = @"240.0 km";
    spot.spotIdentifier = @"HappySpots4Cell";
    [array addObject:spot];
    
    spot = [[__Spot alloc] init];
    spot.place = @"Disney World - 71%";
    spot.location = @"Berlin, Germany";
    spot.imageName = @"faraway.png";
    spot.distance = @"392.0 km";
    spot.spotIdentifier = @"HappySpots3Cell";
    [array addObject:spot];
    
    spot = [[__Spot alloc] init];
    spot.place = @"Disney World - 68%";
    spot.location = @"Berlin, Germany";
    spot.imageName = @"mordor.png";
    spot.distance = @"900.0 km";
    spot.spotIdentifier = @"HappySpots3Cell";
    [array addObject:spot];
    
    spot = [[__Spot alloc] init];
    spot.place = @"Tattoine - 56%";
    spot.location = @"Berlin, Germany";
    spot.imageName = @"tatooine.png";
    spot.distance = @"970.0 km";
    spot.spotIdentifier = @"HappySpots3Cell";
    [array addObject:spot];
    
    spot = [[__Spot alloc] init];
    spot.place = @"Mordor - 34%";
    spot.location = @"Berlin, Germany";
    spot.imageName = @"tortuga.png";
    spot.distance = @"1000.0 km";
    spot.spotIdentifier = @"HappySpots2Cell";
    [array addObject:spot];
    
    return array;
}

@end
