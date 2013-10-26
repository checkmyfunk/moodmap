//
//  ММWatsYourMoodViewController.m
//  moodmap
//
//  Created by Roman Putsykovich on 10/26/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import "ММWatsYourMoodViewController.h"

@interface __WatsYourMoodViewController ()

@property (strong, nonatomic) NSArray *data;
@property (strong, nonatomic) NSIndexPath *checkedIndexPath;

@end

@implementation __WatsYourMoodViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    self.data = @[@"Feeling Awesome",@"Feeling Bad", @"Feeling Loved", @"Feeling Sleepy", @"Feeling Pretty", @"Feeling Determined"];
    self.checkedIndexPath = [NSIndexPath indexPathForRow:0 inSection:0];
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [self.data count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
        static NSString *CellIdentifier = @"MoodCell";
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier forIndexPath:indexPath];
        
        if (!cell) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
        }
        
        cell.accessoryType = indexPath.row == self.checkedIndexPath.row ? UITableViewCellAccessoryCheckmark : UITableViewCellAccessoryNone;
        cell.textLabel.text = [self.data objectAtIndex:indexPath.row];
        return cell;
}


#pragma mark - Table view delegate

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    if (indexPath.row != self.checkedIndexPath.row) {
        UITableViewCell *newCell = [tableView cellForRowAtIndexPath:indexPath];
        newCell.accessoryType = UITableViewCellAccessoryCheckmark;
        
        UITableViewCell *oldCell = [tableView cellForRowAtIndexPath:self.checkedIndexPath];
        oldCell.accessoryType = UITableViewCellAccessoryNone;
        
        self.checkedIndexPath = indexPath;
    }
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
